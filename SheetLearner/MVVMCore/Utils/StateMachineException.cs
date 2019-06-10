using System;
using System.Runtime.Serialization;

namespace MVVMCore.Utils
{
	[Serializable]
	internal class StateMachineException : Exception
	{
		public StateMachineException()
		{
		}

		public StateMachineException(string message) : base(message)
		{
		}

		public StateMachineException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected StateMachineException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}