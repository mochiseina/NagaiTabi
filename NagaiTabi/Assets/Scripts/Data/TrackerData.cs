using System;
using System.Collections.Generic;

[Serializable]
public class TrackerData
{
	public string playerName = "";
	public List<ImmersionEntry> entries = new();
}