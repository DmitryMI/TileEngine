using System;

namespace Assets.Scripts.GameMechanics.Chemistry.Reactions
{
    class Reaction
    {
        public static void React(SubstanceMixture mixture)
        {
            foreach (var reaction in AvailableReactions)
            {
                int[] reagentMixtureIndexes;

                bool allReagentsArePresent = FindReagentIndexes(mixture, reaction, out reagentMixtureIndexes);

                if (allReagentsArePresent)
                {
                    float[] reagentVolumes = GetReagentVolumes(mixture, reagentMixtureIndexes);

                    float[] factors = GetFactors(reagentVolumes, reaction.Reagents);

                    int minFactorIndex = FindMinIndex(factors);

                    float reactionFactor = factors[minFactorIndex];

                    DoReaction(mixture, reaction, reagentMixtureIndexes, reactionFactor);
                }
            }
        }

        private static void DoReaction(SubstanceMixture mixture, Reaction reaction, int[] reagentsIndexes, float factor)
        {
            for (int i = 0; i < reagentsIndexes.Length; i++)
            {
                int reagentIndex = reagentsIndexes[i];
                if (reagentIndex < 0 || reagentIndex > mixture.Count)
                {
                    return;
                }
                SubstanceInfo substanceInfo = mixture[reagentIndex];
                float reactionVolume = factor * reaction.Reagents[i].Mole;
                substanceInfo.Volume -= reactionVolume;

                if (substanceInfo.Volume > 0)
                {
                    mixture[reagentIndex] = substanceInfo;
                }
                else
                {
                    mixture.RemoveAt(reagentIndex);
                }
            }

            SubstanceMixture additions = new SubstanceMixture(0, mixture.Temperature + reaction.TermalFactor * factor);

            for (int i = 0; i < reaction.Results.Length; i++)
            {
                int id = ChemistryController.Current.GetSubstance(reaction.Results[i].SubstanceName).Id;
                if (id != -1) 
                    additions.Add(new SubstanceInfo(id, factor * reaction.Results[i].Mole));
            }

            mixture.Concatinate(additions);
        }

        private static float[] GetFactors(float[] reagentVolumes, Reagent[] reagents)
        {
            float[] factors = new float[reagentVolumes.Length];

            for (int i = 0; i < factors.Length; i++)
            {
                factors[i] = reagentVolumes[i] / reagents[i].Mole;
            }

            return factors;
        }

        private static int FindMinIndex<T>(T[] floats) where T: IComparable
        {
            int min = 0;
            for (int i = 0; i < floats.Length; i++)
                if (floats[i].CompareTo(floats[min]) == -1)
                    min = i;

            return min;
        }

        private static float[] GetReagentVolumes(SubstanceMixture mixture, int[] indexes)
        {
            float[] reagentVolumes = new float[indexes.Length];

            for (int i = 0; i < indexes.Length; i++)
                reagentVolumes[i] = mixture[indexes[i]].Volume;

            return reagentVolumes;
        }

        private static int FindMaxMoleReagent(Reaction reaction)
        {
            int maxMoleIndex = 0;

            for (int i = 1; i < reaction.Reagents.Length; i++)
            {
                if (reaction.Reagents[i].Mole > reaction.Reagents[maxMoleIndex].Mole)
                    maxMoleIndex = i;
            }

            return maxMoleIndex;
        }

        private static bool FindReagentIndexes(SubstanceMixture mixture, Reaction reaction, out int[] reagentIndexes)
        {
            reagentIndexes = new int[reaction.Reagents.Length];

            bool ok = true;

            for (int i = 0; i < reagentIndexes.Length && ok; i++)
            {
                //string reagentName = reaction.Reagents[i].SubstanceName;
                int reagentId = ChemistryController.Current.GetSubstance(reaction.Reagents[i].SubstanceName).Id;
                int index = FindSubstanceIndex(mixture, reagentId);

                if (index != -1)
                {
                    reagentIndexes[i] = index;
                }
                else
                {
                    ok = false;
                }
            }

            return ok;
        }

        private static int FindSubstanceIndex(SubstanceMixture mixture, string name)
        {
            for (int i = 0; i < mixture.Count; i++)
            {
                if (ChemistryController.Current.GetSubstance(mixture[i].SubstanceId).Name == name)
                    return i;
            }

            return -1;
        }

        private static int FindSubstanceIndex(SubstanceMixture mixture, int id)
        {
            for (int i = 0; i < mixture.Count; i++)
            {
                if (mixture[i].SubstanceId == id)
                    return i;
            }

            return -1;
        }

        private static Reaction[] AvailableReactions =
        {
            new Reaction(
                new[]
                {
                    new Reagent("Water", 1), new Reagent("Calcium Oxide", 1) // Source reagents
                }, 
                new[]
                {
                    new Reagent("Calcium Hydroxide", 2) // Resulting reagents
                }),

            new Reaction(
                new[]
                {
                    new Reagent("Hydrogen", 3), new Reagent("Nitrogen", 1) // Source reagents
                },
                new[]
                {
                    new Reagent("Ammonia", 3) // Resulting reagents
                }),
        };

        public Reaction(Reagent[] reagents, Reagent[] results, float minTemp = 0)
        {
            Reagents = reagents;
            Results = results;
            MinimalTemperature = minTemp;
        }

        public Reagent[] Reagents;
        public Reagent[] Results;
        public float MinimalTemperature;
        public float TermalFactor;
    }
}
