using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class VoiceRecognitionRepeating : MonoBehaviour {

	public bool CanRecord;
	private bool dontRecord;
	private float notTouchTime;
	private int myTimeSamples;
	[Range(0.0001f,0.001f)]
	public float averageVolumeBuffer;
	private const float THRESHOLD = 0.02f; 
	private float rmsValue;            // Volume in RMS
	private float dbValue;             // Volume in DB
	private float pitchValue;  
	private float[] spectrum ;  
	int samplerate = 44100;

	public int clamp = 160;
	public AudioClip myClip;
	public float volume,lastVolume;
	public string idleAnim,talkAnim;
	public float timerSinceMicrophoneStarted;

	private static float REFVALUE  = 0.1f;  
	private static int SAMPLECOUNT = 1024;   // Sample Count.
	private Character MainCharacter;

    public static VoiceRecognitionRepeating Instance;

    private void Awake()
    {
		MainCharacter = GetComponent<Character>();
		Instance = this;
    }
    void Start()
	{
		Application.targetFrameRate = 60;
		spectrum = new float[8192];
	}

	private void Update()
	{
        timerSinceMicrophoneStarted += Time.deltaTime;

        if (timerSinceMicrophoneStarted > 51f && !dontRecord && !MainCharacter.CurrentAnimation.Equals(talkAnim))
        {
            Debug.Log("record again after 50 sec");
            Record();
        }
		/*
#if UNITY_EDITOR

#else

		if (Input.touchCount > 0)
		{
			if(!dontRecord)
			{
				AnimationHandler._instance.audioPlayingSource.mute = true;
				if(MainCharacter.CurrentAnimation.Equals(talkAnim))
				{
					StopCoroutine (WaitingForTalkToStop());
				}
				if(MainCharacter.CurrentAnimation.Equals("Listen"))
				{
					StopListen();
				}
				checkingWhenToStop = false;
				StopCoroutine(CheckWhenToStop());
			}
			dontRecord = true;
			notTouchTime = 0;
		}
		else
		{
			if (dontRecord) 
			{
				notTouchTime+=Time.deltaTime;

				if (notTouchTime > 1.0f)
				{
					if((MainCharacter.CurrentAnimation.Equals(idleAnim)) && !AnimationHandler._instance.GetComponent<AudioSource>().isPlaying)
					{
						SubsequentRecord();
						dontRecord = false;
					}
					
				}
				
			}
		}
#endif
		*/
	}
	/// <summary>
	/// Stops the microphone
	/// </summary>
	public void StopIt()
	{
		CanRecord = false;
        checkingWhenToStop = false;
		StopCoroutine(CheckWhenToStop());
		StopCoroutine (WaitingForTalkToStop());
		StopAllCoroutines();
		CancelInvoke(nameof(RecordAgain));
        MainCharacter.PlayAnimation(idleAnim);
        AnimationHandler._instance.audioPlayingSource.Stop();
		AnimationHandler._instance.audioPlayingSource.clip = null;
    }

    public void RecordAgain()
	{
		StartCoroutine(CheckWhenToStop());
	}
	int  minFreq,maxFreq;
	/// <summary>
	/// Record this audio.
	/// </summary>
	public void Record()
	{
		if (CanRecord)
		{
			GetComponent<AudioSource>().timeSamples = 0;
            myTimeSamples = 0;
            lastSample = 0;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
            if ((minFreq + maxFreq) == 0)//These 2 lines of code are mainly for windows computers
                maxFreq = 44100;
            GetComponent<AudioSource>().clip = Microphone.Start(null, false, 50, maxFreq);
            iPhoneSpeaker.ForceToSpeaker();
            while (!(Microphone.GetPosition(null) > 0))
            {
            }
            iPhoneSpeaker.ForceToSpeaker();
            GetComponent<AudioSource>().Play();
            StartCoroutine(CheckWhenToStop());
        }
        timerSinceMicrophoneStarted = 0;
    }
	public void SubsequentRecord()
	{
		StartCoroutine(ActiveRecord(1f));

		IEnumerator ActiveRecord(float Timer)
        {
			yield return new WaitForSeconds(Timer);

			dontRecord = false;
			StartCoroutine(CheckWhenToStop());
		}
	}
	void StopListen()
	{
		if (MainCharacter.CurrentAnimation.Equals("Listen"))
        {
			MainCharacter.PlayAnimation(idleAnim);
		}
	}

	bool checkingWhenToStop;
	int finalSample;
	int lastSample;
	/// <summary>
	/// Checks  when the audio should stop.
	/// </summary>
	/// <returns>The when to stop.</returns>
	IEnumerator CheckWhenToStop()
	{
		if(!checkingWhenToStop)
		{
			checkingWhenToStop = true;
			float timer = 0.0f;
			float clipTime = 0.01f;
			int consecutiveNullVoice = 0;
			bool onceZero = false;
			bool gotASound = false;
			myTimeSamples = GetComponent<AudioSource>().timeSamples;
			lastSample = GetComponent<AudioSource>().timeSamples;;
			while(consecutiveNullVoice < 11)
			{
				while( (GetAveragedVolume() > averageVolumeBuffer) && !AnimationHandler._instance.GetComponent<AudioSource>().isPlaying)
				{
					consecutiveNullVoice = 0;
					if(!gotASound)
					{
                        MainCharacter.PlayAnimation("Listen");
                        clipTime = 0.01f;
					}
					while(timer < 0.08f)
					{
						timer+=0.02f;
						clipTime+=0.02f;
						yield return 0;
					}
					finalSample = GetComponent<AudioSource>().timeSamples;
					gotASound = true;
					onceZero = false;
					timer = 0.0f;

					yield return 0;
				}
				if(!gotASound)
				{
					lastSample = myTimeSamples;
					myTimeSamples = GetComponent<AudioSource>().timeSamples;
				}
				if(onceZero)
				{
					float timerTocheck = 0.08f;
					consecutiveNullVoice++;
					timer = 0.0f;
					while(timer < timerTocheck)
					{
						timer+=0.02f;
						clipTime+=0.02f;
						yield return 0;
					}
					timer = 0.0f;
				}
				onceZero = true;
				yield return 0;

			}
			checkingWhenToStop = false;
			if(!dontRecord)
			{
				if(clipTime > 0.905f && !AnimationHandler._instance.GetComponent<AudioSource>().isPlaying && gotASound)
				{
					finalSample = GetComponent<AudioSource>().timeSamples;
					PlaySound (clipTime);
				}
				else
				{
					if (!AnimationHandler._instance.GetComponent<AudioSource>().isPlaying)
					{
						MainCharacter.PlayAnimation(idleAnim);
						SubsequentRecord();
					}
				}
			}

		}
	}
	/// <summary>
	/// Plays the sound.
	/// </summary>
	/// <param name="clipTime">Clip time.</param>
	public void PlaySound(float clipTime)
	{
		AnimationHandler._instance.audioPlayingSource.clip = GetComponent<AudioSource>().clip;
		AnimationHandler._instance.audioPlayingSource.timeSamples = lastSample;
		AnimationHandler._instance.audioPlayingSource.pitch = 1.6f;
		AnimationHandler._instance.audioPlayingSource.mute = false;
		AnimationHandler._instance.audioPlayingSource.Play ();
		MainCharacter.PlayAnimation(talkAnim);
		StartCoroutine(WaitingForTalkToStop());
	}
	IEnumerator WaitingForTalkToStop()
	{
		while(AnimationHandler._instance.audioPlayingSource.timeSamples < (finalSample-5000))
		{
			yield return 0;
		}
		MuteIt();
	}
	void MuteIt()
	{
		AnimationHandler._instance.audioPlayingSource.Stop ();
		AnimationHandler._instance.audioPlayingSource.clip = null;
		MainCharacter.PlayAnimation(idleAnim);
		Invoke(nameof(RecordAgain), 0.4f); 
	}
	void OnApplicationPause(bool isPaused)
	{
		Screen.orientation = ScreenOrientation.Portrait;
	}
	void ChangeData()
	{
		float[] samples = new float[GetComponent<AudioSource>().clip.samples * GetComponent<AudioSource>().clip.channels];
		GetComponent<AudioSource>().clip.GetData(samples, 0);
		int i = 0;
		while (i < samples.Length) {
			samples[i] = samples[i] * 1.5F;
			++i;
		}
		GetComponent<AudioSource>().clip.SetData(samples, 0);
	}
	float LastSamplesVolume()
	{ 
		float[] data = new float[512];
		float a = 0;
		GetComponent<AudioSource>().GetOutputData(data,0);
		
		for(int i = 511;i< 512;i++)
		{
			a += Mathf.Abs(data[i]);
		}
		volume = (a/1);
		return a/1;
	}
	float GetAveragedVolume()
	{ 
		float[] data = new float[256];
		float a = 0;
		GetComponent<AudioSource>().GetOutputData(data,0);
		
		foreach(float s in data)
		{
			a += Mathf.Abs(s);
		}
		volume = (a/256);
		return a/256;
		
	}
	void AnalyzeSound()
	{
		// Get all of our samples from the mic.
		float []samples = new float[SAMPLECOUNT];
		GetComponent<AudioSource>().GetOutputData(samples, 0);
		// Sums squared samples
		float sum =  0;
		for (int i = 0; i < SAMPLECOUNT; i++) {
			sum += Mathf.Pow(samples[i], 2);
		}
		
		// RMS is the square root of the average value of the samples.
		// Used to calculate the volume in dB
		rmsValue = Mathf.Sqrt (sum / SAMPLECOUNT);
		dbValue = 20 * Mathf.Log10 (rmsValue / REFVALUE);
		
		// Clamp it to {clamp} min
		if (dbValue < -clamp) {
			dbValue = -clamp;
		}
		
		// Gets the sound spectrum.
		GetComponent<AudioSource>().GetSpectrumData (spectrum, 0, FFTWindow.BlackmanHarris);
		float maxV = 0;
		int maxN = 0;
		
		// Find the highest sample.
		for (int j = 0; j < SAMPLECOUNT; j++) {
			if (spectrum [j] > maxV && spectrum [j] > THRESHOLD) {
				maxV = spectrum [j];
				maxN = j; // maxN is the index of max
			}
		}
		
		// Pass the index to a float variable
		float freqN = maxN;
		
		// Interpolate index using neighbours
		if (maxN > 0 && maxN < SAMPLECOUNT - 1) {
			float dL = spectrum [maxN - 1] / spectrum [maxN];
			float dR = spectrum [maxN + 1] / spectrum [maxN];
			freqN += 0.5f * (dR * dR - dL * dL);
		}
		
		// Convert index to frequency
		pitchValue = freqN * 24000 / SAMPLECOUNT;
		Debug.Log("pitch = "+pitchValue);
	}
	float GetFundamentalFrequency()
	{
		float fundamentalFrequency = 0.0f;
		float[] data = new float[8192];
		GetComponent<AudioSource>().GetSpectrumData(data,0,FFTWindow.BlackmanHarris);
		float s = 0.0f;
		int i = 0;
		for (int j = 1; j < 8192; j++)
		{
			if ( s < data[j] )
			{
				s = data[j];
				i = j;
			}
		}
		fundamentalFrequency = i * samplerate / 8192;
		return fundamentalFrequency;
	}
}
