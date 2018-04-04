using System;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class UtilsTest {

	[Test]
	public void ProbabilityContainerTest()
	{
		ProbabilityContainer<string> stuff = new ProbabilityContainer<string>();
        stuff.AddItem("Common", 100);
	    stuff.AddItem("Common2", 100);
        stuff.AddItem("Uncommon", 10);
	    stuff.AddItem("Rare", 1);
	    var counts = new Dictionary<string, int>();
	    counts["Common"] = 0;
        counts["Common2"] = 0;
	    counts["Uncommon"] = 0;
	    counts["Rare"] = 0;
        for (int i = 0; i < 10000; i++)
        {
            var thing = stuff.GetRandom();
            counts[thing]++;
        }

        // Console.WriteLine("Common: " + counts["Common"]);
	    // Console.WriteLine("Common2: " + counts["Common2"]);
        // Console.WriteLine("Uncommon: " + counts["Uncommon"]);
        // Console.WriteLine("Rare: " + counts["Rare"]);

        Assert.IsTrue(counts["Common"] > 0);
	    Assert.IsTrue(counts["Common2"] > 0);
	    Assert.IsTrue(counts["Uncommon"] > 0);
	    Assert.IsTrue(counts["Rare"] > 0);
	    Assert.IsTrue(counts["Common"] > counts["Uncommon"]);
	    Assert.IsTrue(counts["Common2"] > counts["Uncommon"]);
	    Assert.IsTrue(counts["Uncommon"] > counts["Rare"]);
    }

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator UtilsTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
