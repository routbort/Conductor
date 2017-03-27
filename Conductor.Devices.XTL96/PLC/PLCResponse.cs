using System;

namespace Conductor.Devices.XTL96
{
	public class PLCResponse
	{
		private readonly string response;

		private readonly PLCResponseType plcResponseType;

		private readonly DateTime timestamp;

		public string Message
		{
			get
			{
				return this.response;
			}
		}

		public PLCResponseType PLCResponseType
		{
			get
			{
				return this.plcResponseType;
			}
		}

		public DateTime Timestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		public PLCResponse(string response)
		{
			this.response = response;
			this.plcResponseType = PLCResponse.MapResponse(response);
			this.timestamp = DateTime.Now;
		}

		private static PLCResponseType MapResponse(string message)
		{
			PLCResponseType pLCResponseType;
			if (message.ToLower() == "ok")
			{
				pLCResponseType = PLCResponseType.Ok;
			}
			else if (message.ToLower() == "posok")
			{
				pLCResponseType = PLCResponseType.PosOk;
			}
			else if (message.ToLower() == "printjobcomplete")
			{
				pLCResponseType = PLCResponseType.PrintjobOk;
			}
			else if (message.ToLower() == "vacuumorprinterror")
			{
				pLCResponseType = PLCResponseType.PrintjobError;
			}
			else if (message.ToLower() == "tubeupcylerr")
			{
				pLCResponseType = PLCResponseType.TubeupError;
			}
			else if (message.ToLower() == "scannok")
			{
				pLCResponseType = PLCResponseType.ScannOk;
			}
			else if (message.ToLower() == "inloadposok")
			{
				pLCResponseType = PLCResponseType.InLoadPosOk;
			}
			else if (!(message.ToLower() == "scannererror"))
			{
				pLCResponseType = (!(message.ToLower() == "statusok") ? PLCResponseType.UnknownMessage : PLCResponseType.StatusOk);
			}
			else
			{
				pLCResponseType = PLCResponseType.ScannerError;
			}
			return pLCResponseType;
		}
	}
}