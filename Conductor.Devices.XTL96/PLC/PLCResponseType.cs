using System;

namespace Conductor.Devices.XTL96
{
	public enum PLCResponseType
	{
		Ok,
		PrintjobOk,
		PrintjobError,
		TubeupError,
		UnknownMessage,
		ScannOk,
		InLoadPosOk,
		ScannerError,
		StatusOk,
		PosOk
	}
}