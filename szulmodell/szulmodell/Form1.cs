﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using szulmodell.Entities;

namespace szulmodell
{
    public partial class Form1 : Form
    {
        List<Person> Population = new List<Person>();
        List<BirthProbability> BirthProbabilities = new List<BirthProbability>();
        List<DeathProbability> DeathProbabilities = new List<DeathProbability>();
        List<int> Male = new List<int>();
        List<int> Female = new List<int>();

        Random rng = new Random(1234);
        public Form1()
        {
            InitializeComponent();
        }
        public List<Person> GetPopulation(string csvpath)
        {
            List<Person> population = new List<Person>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    population.Add(new Person()
                    {
                        BirthYear = int.Parse(line[0]),
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[1]),
                        NbrOfChildren = int.Parse(line[2])
                    });
                }
            }

            return population;
        }

        public List<BirthProbability> GetBirthProbabilities(string csvpath)
        {
            List<BirthProbability> births = new List<BirthProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    births.Add(new BirthProbability()
                    {
                        Age = int.Parse(line[0]),
                        ChildNum=int.Parse(line[1]),
                        P=double.Parse(line[2])
                    }) ;
                }
            }
            return births;
        }

        public List<DeathProbability> GetDeathProbabilities(string csvpath)
        {
            List<DeathProbability> deaths = new List<DeathProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    deaths.Add(new DeathProbability()
                    {
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[0]),
                        Age = int.Parse(line[1]),
                        P = double.Parse(line[2])
                    });
                }
            }
            return deaths;
        }

        private void SimStep(int year, Person person)
        {
            if (!person.IsAlive) return;

            byte age = (byte)(year - person.BirthYear);

            double pDeath = (from x in DeathProbabilities
                             where x.Gender == person.Gender && x.Age == age
                             select x.P).FirstOrDefault();
            
            if (rng.NextDouble() <= pDeath)
                person.IsAlive = false;

            if (person.IsAlive && person.Gender == Gender.Female)
            {
                double pBirth = (from x in BirthProbabilities
                                 where x.Age == age
                                 select x.P).FirstOrDefault();
                
                if (rng.NextDouble() <= pBirth)
                {
                    Person újszülött = new Person();
                    újszülött.BirthYear = year;
                    újszülött.NbrOfChildren = 0;
                    újszülött.Gender = (Gender)(rng.Next(1, 3));
                    Population.Add(újszülött);
                }
            }
        }


        private void Simulation()
        {
            Population = GetPopulation(textBox1.Text);
            BirthProbabilities = GetBirthProbabilities(@"C:\Temp\születés.csv");
            DeathProbabilities = GetDeathProbabilities(@"C:\Temp\halál.csv");

            for (int year = 2005; year <= numericUpDown1.Value; year++)
            {
                for (int i = 0; i < Population.Count; i++)
                {
                    SimStep(year, Population[i]);
                }

                int nbrOfMales = (from x in Population
                                  where x.Gender == Gender.Male && x.IsAlive
                                  select x).Count();
                Male.Add(nbrOfMales);
                int nbrOfFemales = (from x in Population
                                    where x.Gender == Gender.Female && x.IsAlive
                                    select x).Count();
                Female.Add(nbrOfFemales);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Male.Clear();
            Female.Clear();
            richTextBox1.Clear();
            Simulation();
            DisplayResult();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK) return;
            textBox1.Text = ofd.FileName;
        }

        private void DisplayResult()
        {
            for (int year = 2005; year <= numericUpDown1.Value; year++)
            {
                richTextBox1.AppendText(string.Format("Szimulációs év:{0} \n \t Fiúk:{1} \n \t Lányok:{2} \n", year, Male[year - 2005], Female[year - 2005]));
            }
        }
    }
}
