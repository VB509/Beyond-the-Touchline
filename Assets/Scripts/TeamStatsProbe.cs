
using UnityEngine;
using Fungus;

public class TeamStatsProbe : MonoBehaviour
{
    public Flowchart flowchart;

    private int lastAttack;
    private int lastDefense;
    private int lastFitness;
    private bool initialized = false;

    void Update()
    {
        if (flowchart == null)
            return;

        int attack = flowchart.GetIntegerVariable("TeamAttack");
        int defense = flowchart.GetIntegerVariable("TeamDefense");
        int fitness = flowchart.GetIntegerVariable("TeamFitness");

        if (!initialized ||
            attack != lastAttack ||
            defense != lastDefense ||
            fitness != lastFitness)
        {
            Debug.Log(
                $"TEAM STATS | Attack: {attack} | " +
                $"Defense: {defense} | Fitness: {fitness}"
            );

            lastAttack = attack;
            lastDefense = defense;
            lastFitness = fitness;
            initialized = true;
        }
    }
}
