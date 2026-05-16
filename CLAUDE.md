# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

`3230Project1` (repo: SNPBreastCancer) is a .NET Framework 4.5 C# console application — an academic project from 2014. It estimates breast-cancer risk from raw consumer-DNA data by cross-referencing SNPs in an AncestryDNA raw-data export against a curated set of risk alleles in known breast-cancer genes (ATM, BRCA1, BRCA2, TP53, CHEK2).

## Build & Run

- Solution `3230Project1.sln` is in Visual Studio 2013 format. Build with Visual Studio or `msbuild 3230Project1.sln`. Output: `3230Project1/bin/Debug/3230Project1.exe`.
- No `dotnet` / `msbuild` / `mono` toolchain is installed in this environment, so code changes cannot be compiled or run here — review changes by reading the code.
- There is no test suite and no linter configured.
- `Program.cs` opens its data files with paths relative to the executable: `../../input.txt` and `../../riskalleles.xml`. These resolve to the `3230Project1/` project directory only when the program runs from its default `bin/Debug/` (or `bin/Release/`) output folder.

## Architecture

A single pass in `Program.Main`:

1. `Util.ReadFile` loads the entire AncestryDNA export (`input.txt`, TAB-delimited: rsid, chromosome, position, allele1, allele2) into a `string[]`.
2. `riskalleles.xml` is parsed with LINQ-to-XML. Each `<g>` position element becomes an `allele` object (`allele.cs`). `allele.Reorient()` then rewrites every risk genotype onto the `+` strand.
3. The DNA rows are scanned; each SNP line is matched by chromosome then `rsid` to an `allele`, whose `setSampleGenotype` is called.
4. `allele.Risk` (a getter backed by `assessRisk()`) returns the risk magnitude for the matched genotype; results are written to the console.

`allele` is the core domain type: one SNP "position of interest", holding its risk genotypes and per-genotype risk magnitudes, the sample's observed genotype, strand orientation, and the `assessRisk()` calculation. `Util` only provides file I/O helpers.

## riskalleles.xml — the risk model

Structure: root `<DNA>` → gene elements (`<ATM>`, `<BRCA1>`, …) each with a `chromosome` attribute → `<g>` position elements (`rsid`, `orientation`, `s`) → `<a>` risk-allele elements with an optional `m` magnitude attribute.

- `s` = statistical significance: `0` first calculation method only, `1` first method + display, `2` first + second method.
- An `<a>` with a single allele (`C`) implies magnitude 1 per allele (max 2). A `;`-delimited genotype (`C;T`) must carry an `m` attribute.
- `orientation="-"` genotypes are complemented to `+` by `allele.Reorient()`.
- The file MUST be pre-sorted ascending by chromosome then position; the code depends on this ordering and never sorts.

## Hardcoded sizes (coupled across files)

Two array sizes are hardcoded to the current data and must be changed together when the data changes:

- `Util.ReadFile` preallocates a `string[701496]`, sized to the current `input.txt` (701,495 lines). A larger input file throws `IndexOutOfRangeException`.
- `Program.cs` allocates `allele[25]`, sized to the 25 `<g>` positions currently in `riskalleles.xml`. Adding positions overflows the array.

## Known limitations

- The parsing loop filters DNA lines by the chromosome of `alleles[POIPointer]`, but `POIPointer` is never advanced (the increment is commented out), so it stays `0`. In practice only chromosome 11 (ATM) SNPs are processed; BRCA2/TP53/BRCA1/CHEK2 lines are skipped.
- The program prints per-SNP risk magnitudes only; it does not compute the aggregate/cumulative risk or use the `s` significance levels described in the XML header comment.
