using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Data;
namespace _3230Project1
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //int indexOfFirstDNA=-1;                          //does not change
            int indexOfDNA = 0;                               //once code reaches DNA within ChrisDNA.txt, acts as counter for current SNP within text file
            int POIPointer = 0;                             //pointer to the lowest possible chromosome to search for
            allele[] alleles = new allele[25];              //data structure containing allele locations relative to the statistics
            string[] columns = new string[5];               //array of column headings received from the text file
            string[] fields;                                //array of parsed columns within the current row
            
            string[] lines = Util.ReadFile("../../input.txt");  //read in lines from text file to lines[]
            string[,] table = new string[alleles.Length,columns.Length];   //initialize table to proper dimensions

            /* Load XML file containing gene locations and negatively associated alleles. Genes must specify the Gene locations may
                specify a magnitude, which multiplies the risk.*/
            XDocument docAlleles = XDocument.Load("../../riskalleles.xml");//pull in the xml containing the genes and corresponding bad alleles

            IEnumerable<XElement> genes = from SNP in docAlleles.Root.Elements() select SNP;

            int aCount = 0;                                 //pointer to next available index in alleles[]
            foreach (XElement g in genes)                   //create the allele objects from XML root = <DNA></DNA>
            {//gene level e.g.<BRCA1></BRCA1>; <BRCA2></BRCA2>
                foreach (XElement position in g.Descendants("g"))   //position level e.g.<g></g>
                {
                    //Console.WriteLine(i.Attribute("rsid").Value + " / " + i.Parent.Name + " / " + e.Attribute("chromosome").Value + " / " + i.Attribute("position").Value);
                    alleles[aCount] = new allele(position.Attribute("rsid").Value, 
                        g.Name.ToString(), g.Attribute("chromosome").Value, 
                        position.Attribute("orientation").Value, 
                        position.Attribute("s").Value);
                    
                    foreach (XElement a in position.Elements("a"))
                    {
                        string[] sampleAlleles = a.Value.Split(new char[] { ';' });
                        if (sampleAlleles.Length == 1)            //if only 1 allele is given
                        {
                            alleles[aCount].addRiskGenotype(a.Value);
                        }
                        else                                        //else 2 alleles given; expect a.m attribute; use full array of genotype
                        {
                            
                            alleles[aCount].addRiskGenotype(sampleAlleles, a.Attribute("m").Value);
                        }
                       

                    }
                    alleles[aCount].Reorient();                     //align riskGenotypes to '+' orientation
                    aCount++;
                }
                //genes are isolated to 1 chromosome, therefor I can sort(asc.)  here by position within a particular gene,
                // as long as the genes are in descending chromosomal order within XML. (not strict XML style; order matters)
                    /* alternatively, I used a presorted XML file to eliminate O(nlgn) sort here. */
                //Console.WriteLine(genes.ElementAt<XElement>(aCount).Descendants("rsid").ToString());
            }


            ////////////////////////


            for (int i = 0; i < lines.Length; i++)          //this loop finds the line containing column headings, then passes control to loop which parses DNA
            {
                if(lines[i].ElementAt<char>(0)!='#' && lines[i].ElementAt<char>(2)=='i')//line is not a comment, nor contains a valid rsid
                {                                           // line i must be column headings
                    indexOfDNA=i;                           //set it and leave it for calculation of number of iterations of upcoming loop
                    //indexOfFirstDNA = i+1;                   //lines[i] must contain column headings; set firstLineOfDNA to indicate the upcoming line is data
                    columns = lines[i].Split(new char[]{ '\t' });//split the line to obtain column headings
                    break;
                }
            }
            do                                              //this loop begins parses DNA of particular set of columns found at lines[lineOfFirstDNA]
            {                                                //it is implied that line occurs after column headings.
                indexOfDNA++;
                if (lines[indexOfDNA] != null && lines[indexOfDNA].ElementAt<char>(0) == 'r')         // Data validation: if line begins with 'r' then is most likely a SNP
                {
                    fields = lines[indexOfDNA].Split(new char[] { '\t' });//split the fields over '\t'
                    if (Int32.Parse(fields[1]) != alleles[POIPointer].getChromosomeLocation()) //if we haven't reached chromosome, skip iteration
                    {
                        continue;
                    }
                    /*if (fields[0]!=alleles[POIPointer].getID()) //if we haven't reached position, skip iteration
                    {
                        continue;
                    }*/






                    for (int i = 0; i < alleles.Length; i++)
                    {
                        if (fields[0].Equals(alleles[i].getID()))
                        {
                            Console.WriteLine(lines[indexOfDNA] + " / " + indexOfDNA);
                            alleles[i].setSampleGenotype(new string[] { fields[3], fields[4] });
                            Console.WriteLine(alleles[i].Risk + " / " + alleles[i].getRiskAllele());
                            //POIPointer++;                               //found position and analyzed data
                            continue;
                        }
                        /*else if (Int32.Parse(fields[1]) > alleles[POIPointer].getChromosomeLocation())//safe to skip chromosomes less than current
                        {
                            POIPointer=i;//POIPointer contains the lowest possible chromosome to search for
                            continue;
                        } */
                    }

                }
            } while (/*POIPointer < alleles.Length && */lines[indexOfDNA] != null);

            //Match matchDNA;
            //matchDNA = matchDNA = Regex.Match(lines[lineOfDNA], @"^rs\d+\t");

             
            Console.ReadKey();
        }
    }
}
