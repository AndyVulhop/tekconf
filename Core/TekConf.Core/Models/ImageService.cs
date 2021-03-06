﻿using System;
using System.IO;
using System.Net;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.Network.Reachability;

namespace TekConf.Core.Models
{
	public interface IImageService
	{

	}

	public class ImageService : IImageService
	{
		private readonly IMvxFileStore _fileStore;
		private readonly IMvxReachability _reachability;
		private readonly Action<byte[]> _success;
		private readonly Action<Exception> _error;
		private string _imageUrl;

		private ImageService(IMvxFileStore fileStore, IMvxReachability reachability, Action<byte[]> success, Action<Exception> error)
		{
			_fileStore = fileStore;
			_reachability = reachability;
			_success = success;
			_error = error;
		}

		public static void GetImageAsync(IMvxFileStore fileStore, IMvxReachability reachability, string imageUrl, Action<byte[]> success, Action<Exception> error)
		{
			MvxAsyncDispatcher.BeginAsync(() => DoAsyncGetImage(fileStore, reachability, imageUrl, success, error));
		}

		private static void DoAsyncGetImage(IMvxFileStore fileStore, IMvxReachability reachability, string imageUrl, Action<byte[]> success, Action<Exception> error)
		{
			var search = new ImageService(fileStore, reachability, success, error);
			search.StartGetImage(imageUrl);
		}

		private void StartGetImage(string imageUrl)
		{
			_imageUrl = imageUrl;

			try
			{
				var localFileName = imageUrl.Replace("http://tekconf.blob.core.windows.net/images/conferences/", "").Replace("http://www.tekconf.com/img/conferences/", "");
				var localFile = localFileName.Replace(".png", ".jpg");

				if (_fileStore.Exists(localFile))
				{
					byte[] image;
					if (_fileStore.TryReadBinaryFile(localFile, out image))
					{
						_success(image);
					}
				}
				else
				{
					var request = (HttpWebRequest)WebRequest.Create(new Uri(imageUrl));
					request.BeginGetResponse(ReadGetImageCallback, request);
				}
			}
			catch (Exception exception)
			{
				_error(exception);
			}
		}

		private void ReadGetImageCallback(IAsyncResult asynchronousResult)
		{
			try
			{
				var request = (HttpWebRequest)asynchronousResult.AsyncState;
				var response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);

				var buffer = new byte[4096];
				using (var responseStream = response.GetResponseStream())
				{
					using (var memoryStream = new MemoryStream())
					{
						int count;
						do
						{
							count = responseStream.Read(buffer, 0, buffer.Length);
							memoryStream.Write(buffer, 0, count);

						} while (count != 0);

						var result = memoryStream.ToArray();

						var localFileName = _imageUrl.Replace("http://tekconf.blob.core.windows.net/images/conferences/", "").Replace("http://www.tekconf.com/img/conferences/", "");
						
						var localFile = localFileName.Replace(".png", ".jpg");
						_fileStore.WriteFile(localFile, result);

						_success(result);

					}
				}
			}
			catch (Exception exception)
			{
				_error(exception);
			}
		}


	}
}
