﻿using System;
using GAF;
using GAF.Operators;
using TstGenetic.WorldCup;
using System.Collections.Generic;

namespace TstGenetic
{
    class Program
    {
        public const int MAX_GENERATIONS = 100;

        public const int INITIAL_POPULATION_SIZE = 1000;
        public const int CHROMOSOME_SIZE = 20;

        public const int ETILISM = 1;
        public const double CROSSOVER_PROBABILITY = 0.9;
        public const double MUTATION_PROBABILITY = 0.05;

        static void Main(string[] args)
        {
            // Initialize the game table, matches and distances
            WorldCupChronogram.Initialize();
            Distances.Initialize();

            // Population, with a chromossome length
            var population = new Population(CHROMOSOME_SIZE);
            
            // Random seed
            var random = new Random();

            // Initialize the population
            for (int i = 0; i < INITIAL_POPULATION_SIZE; i++)
            {
                var chromossome = new Chromosome();
                for (int j = 0; j < CHROMOSOME_SIZE; j++)
                {
                    //  Somedays have more or less games
                    var maxGamesInDays = WorldCupChronogram.Days[j].NumberOfGames;

                    var gene = new Gene(random.Next(maxGamesInDays));

                    chromossome.Add(gene);
                }

                population.Solutions.Add(chromossome);
            }

            // Elite operator
            var elite = new Elite(ETILISM);

            // Crossover operator
            var crossover = new Crossover(CROSSOVER_PROBABILITY);
         
            // Mutation operador
            var mutate = new SwapMutate(MUTATION_PROBABILITY);

            // GA
            var ga = new GeneticAlgorithm(population, CalculateFitness);

            // Add operators
            ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutate);
        }


        private static double CalculateFitness(Chromosome solution)
        {
            // Invalidate genes with more games than the day has
            for (int i = 0; i < solution.Genes.Count; i++)
            {
                var maxGamesInDays = WorldCupChronogram.Days[i].NumberOfGames;
                var value = solution.Genes[i].RealValue;

                if (value >= maxGamesInDays)
                    return 0;
            }

            // Normalize between 0 and 1
            return 1 - solution.TotalDistance() / 100000;
        }

    }
}
