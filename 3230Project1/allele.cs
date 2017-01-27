using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;

namespace _3230Project1
{
    class allele
    {
        public double Risk                                      //contains the calculated risk of sample's genotype once sampleGenotype has been declared
        {
            get
            {
                /*if (Risk == null)
                    return 0d;
                else */
                return assessRisk();
            }
            private set { }
        }
        public int Stat = -1;   //indicates statistical relevance: 0=used in 1st method, 1= use in 1st and display, 2=use in 1st and 2nd
        private int Chromosome;
        private string Orientation;
        private string Rsid;
        private string Gene;
        private string[,] genotypeMagnitudes = new string[2, 3];  //string[a1,a2,Magnitude]  max of 2 (rows of) genotypes and associated risk magnitudes
        private string[] sampleGenotype = new string[2];          //relative risk of sample can be assessed once this has been set


        
        public allele()
        {
            Rsid = "rs";                                        //default values
            Gene = "none";
            Chromosome = 0;                                     //first valid chromosome is 1
            Orientation = "=";
        }
        public allele(string rsid, string gene, string chromosome, string orientation, string stat)
        {
                                                               //if chromosome and position are valid, and
            if (Int32.TryParse(chromosome, out Chromosome) && Int32.TryParse(stat, out Stat) && Chromosome <= 25)//25 unique chromosomes? chrisdna.txt goes to 25...
            {                                                   //if rsid appears valid, set Rsid
                if (rsid.ElementAt<char>(0) == 'r' && rsid.ElementAt<char>(1) == 's')
                {
                    Rsid = rsid;
                    Gene = gene;
                    Orientation = orientation;
                }

            }
            //else new allele();
            //else throw new Exception("Not a valid risk allele.");//testing
        }

        public void addRiskGenotype(string genotype)
        {
            //string[] genotype=genotype.Split(new char[]{';'});
            if (genotype.Length==1)                                      //if only 1 allele is declared, associated m is default = 1
            {
                if (genotypeMagnitudes[0, 0] == null)                  //first genotype not declared
                {
                    genotypeMagnitudes[0, 0] = genotype;
                    //genotypeMagnitudes[0, 1] = alleles[1];
                    genotypeMagnitudes[0, 2] = "1";                    //magnitude not declared;  default is 1
                }
                else                                                //first genotype already declared ...redundant for 1 allele?
                {
                    genotypeMagnitudes[1, 0] = genotype;             //add to second row
                    genotypeMagnitudes[1, 1] = genotype;
                    genotypeMagnitudes[1, 2] = "1";                    //magnitude not declared; default 1
                }
            }
            

        }

        public void addRiskGenotype(string[] genotype, string magnitude)
        {
            double m;                                           //double m is parsed magnitude
            //string[] genotype=genotype.Split(new char[]{';'});   //parsed alleles ( A,C,T,G )

            if (Double.TryParse(magnitude, out m))              //if magnitude is valid double
            {
                if (genotypeMagnitudes[0, 0] == null)           //first genotype not declared
                {
                    genotypeMagnitudes[0, 0] = genotype[0];
                    genotypeMagnitudes[0, 1] = genotype[1];     //possibly null
                    genotypeMagnitudes[0, 2] = magnitude;       //magnitude declared;  default is 1
                    if (genotypeMagnitudes[0,1]==null)
                    {
                        genotypeMagnitudes[1, 0] = genotype[0];
                        genotypeMagnitudes[1, 1] = genotype[0];
                        genotypeMagnitudes[1, 2] = "2";           //homozygous risk mag
                    }
                }
                else                                            //first genotype already declared
                {
                    genotypeMagnitudes[1, 0] = genotype[0];      //add to second row
                    genotypeMagnitudes[1, 1] = genotype[1];     //homozygous
                    genotypeMagnitudes[1, 2] = magnitude;       //magnitude declared
                } 
            }
        }

        public void setSampleGenotype(string[] genotype)
        {
            if (genotype!=null)                                 //if genotype == null then no associated sample genotype found!
                sampleGenotype = new string[2] { genotype[0], genotype[1] };
        }

        public void Reorient()
        {

            if (Orientation == "-")
            {
                string[,] flip = new string[2, 2] { { "A", "T" }, { "C", "G" } };
                for (int i = 0; i < 2; i++)
                {
                    for (int a = 0; a < 2; a++)
                    {
                        if (genotypeMagnitudes[i, a] == flip[0, 0])
                            genotypeMagnitudes[i, a] = flip[0, 1];          //A -> T
                        else if (genotypeMagnitudes[i, a] == flip[0, 1])
                                genotypeMagnitudes[i, a] = flip[0, 0];      //T -> A
                            else if (genotypeMagnitudes[i, a] == flip[1, 0])
                                    genotypeMagnitudes[i, a] = flip[1, 1];  //C -> G
                                else if (genotypeMagnitudes[i, a] == flip[1, 1])
                                        genotypeMagnitudes[i, a] = flip[1, 0]; //G -> C 
                    }
                }
                Orientation = "+";
            }
        }
        private double assessRisk()
        {
            double mag = 0;
                                                                //genes have 0 risk until shown to be of interest
            //string[] currentGeno = genotype.Split(new char[]{';'});
            if (genotypeMagnitudes[0,1] != null)                //if genoMag[0,1] != null then a magnitude is guaranteed to exist
            {
                for (int i = 0; i <= genotypeMagnitudes.GetLowerBound(0); i++)
                {                                                      //if genotype matches a known genotype
                    if ((sampleGenotype[0] == genotypeMagnitudes[i, 0] || sampleGenotype[0] == genotypeMagnitudes[i, 1])
                        /*|| (sampleGenotype[0] == genotypeMagnitudes[i, 1] && sampleGenotype[1] == genotypeMagnitudes[1 - i, 0])*/)
                    {
                        mag= Double.Parse(genotypeMagnitudes[i, 2]);   //return magnitude of associated genotype
                    }
                } 
            }
            else
            {
                if (/*sampleGenotype[0] != sampleGenotype[1] &&*/ (sampleGenotype[0] == genotypeMagnitudes[1,0] || sampleGenotype[1] == genotypeMagnitudes[1,0]))
                {
                    mag= Double.Parse(genotypeMagnitudes[0, 2]);   //return magnitude of associated genotype
                }
            }
            return mag;                                      //completed without matching a genotype known to be at-risk.
            
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public string getID()
        {
            return Rsid;
        }

        public string getRiskAllele()
        {

                return genotypeMagnitudes[1, 0];
            

        }
        public int getChromosomeLocation()
        {
            //int[] location = new int[2]{Chromosome};
            //location[0]=Chromosome;
            //location[1]=ChromosomePosition;
            return Chromosome;
        }
    }
}
