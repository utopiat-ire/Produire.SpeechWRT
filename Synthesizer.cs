using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;

namespace Produire.SpeechWRT
{
	public class 話者 : IProduireClass
	{
		SpeechSynthesizer speech = new SpeechSynthesizer();
		System.Media.SoundPlayer player = new System.Media.SoundPlayer();

		public 話者()
		{
		}

		#region 手順

		[自分が, 動詞("話す", "しゃべる")]
		public void 話す([を]string メッセージ)
		{
			Speak(メッセージ);
			player.PlaySync();
		}

		private void Speak(string メッセージ)
		{
			Task<SpeechSynthesisStream> t1 = null;
			Task.Run(() =>
			{
				t1 = speech.SynthesizeTextToStreamAsync(メッセージ).AsTask<SpeechSynthesisStream>();
			}).Wait();
			SpeechSynthesisStream myStream = t1.Result;

			Stream stream = myStream.AsStreamForRead();
			player.Stream = stream;
		}

		[自分("が"), 動詞("話す", "しゃべる")]
		public void と話す([と]string メッセージ)
		{
			話す(メッセージ);
		}

		[自分("が"), 補語("非同期で"), 動詞("話す", "しゃべる")]
		public void 非同期で話す([を]string メッセージ)
		{
			Speak(メッセージ);
			player.Play();
		}

		[自分("が"), 補語("非同期で"), 動詞("話す", "しゃべる")]
		public void と非同期で話す([と]string メッセージ)
		{
			非同期で話す(メッセージ);
		}

		[自分("を"), 手順("一時停止")]
		public void 一時停止()
		{
			pauseLocation = player.SoundLocation;
			player.Stop();
		}

		string pauseLocation = null;
		[自分("を"), 手順("停止")]
		public void 停止()
		{
			player.Stop();
		}

		[自分("を"), 手順("再開")]
		public void 再開()
		{
			if (pauseLocation != null) player.SoundLocation = pauseLocation;
			player.Play();
		}

		#endregion

		#region 設定項目

		public string 名前
		{
			get { return speech.Voice.DisplayName; }
			set { SelectVoice(value); }
		}

		private void SelectVoice(string value)
		{
			var voices = SpeechSynthesizer.AllVoices;
			foreach (var voice in voices)
			{
				if (voice.Id == value || voice.DisplayName == value)
				{
					speech.Voice = voice;
					break;
				}
			}
		}

		public VoiceGender 性別
		{
			get { return speech.Voice.Gender; }
		}

		public string 言語
		{
			get { return speech.Voice.Language; }
		}

		public double 音量
		{
			get { return speech.Options.AudioVolume; }
			set { speech.Options.AudioVolume = value; }
		}

		public double 速度
		{
			get { return speech.Options.SpeakingRate; }
			set { speech.Options.SpeakingRate = value; }
		}

		public SpeechSynthesizer 元実体
		{
			get { return speech; }
		}

		#endregion
	}

	public class 音声合成 : IProduireStaticClass
	{
		SpeechSynthesizer speech = new SpeechSynthesizer();

		public string[] 話者一覧
		{
			get
			{
				var voices = SpeechSynthesizer.AllVoices;
				string[] result = new string[voices.Count];
				int i = 0;
				foreach (var voice in voices)
				{
					result[i] = voice.DisplayName;
					i++;
				}
				return result;
			}
		}
		public SpeechSynthesizer 元実体
		{
			get { return speech; }
		}

	}
}
