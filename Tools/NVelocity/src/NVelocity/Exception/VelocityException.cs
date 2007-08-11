// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace NVelocity.Exception
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>  
	/// Base class for Velocity exceptions thrown to the
	/// application layer.
	/// </summary>
	[Serializable]
	public class VelocityException : Exception
	{
		public VelocityException(String exceptionMessage) : base(exceptionMessage)
		{
		}

		public VelocityException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected VelocityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}