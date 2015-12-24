using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LogAppender.Formatters
{
	public class TextMediaTypeFormatter : MediaTypeFormatter
	{
		public TextMediaTypeFormatter()
		{
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
			//SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

			SupportedEncodings.Add(new UTF8Encoding(false, true));
			SupportedEncodings.Add(new UnicodeEncoding(false, true, true));
		}

		public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
		{
			var taskSource = new TaskCompletionSource<object>();
			try
			{
				Encoding effectiveEncoding = SelectCharacterEncoding(content.Headers);
				using (var ms = new MemoryStream())
				{
					readStream.CopyTo(ms);
					taskSource.SetResult(effectiveEncoding.GetString(ms.ToArray()));
				}
			}
			catch (Exception e)
			{
				taskSource.SetException(e);
			}
			return taskSource.Task;
		}

		public override bool CanReadType(Type type)
		{
			return type == typeof(string);
		}

		public override bool CanWriteType(Type type)
		{
			return false;
		}
	}
}