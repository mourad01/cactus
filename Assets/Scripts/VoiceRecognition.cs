using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class VoiceRecognition : MonoBehaviour {

	private static int SAMPLECOUNT = 1024;   // Sample Count.
	private float rmsValue;            // Volume in RMS
	private float dbValue;             // Volume in DB
	private float pitchValue;  
	private float[] spectrum ;  
	private static float REFVALUE  = 0.1f;  
	public int clamp = 160;
	private const float THRESHOLD = 0.02f; 
	public AudioClip myClip;
	public float volume,lastVolume;
	float averageVolumeBuffer;
	void Start() {
#if UNITY_EDITOR
		averageVolumeBuffer = 0.08f;
#else
		averageVolumeBuffer = 0.08f;
#endif
		spectrum = new float[8192];
//		audio.clip = Microphone.Start(null, false, 2, 44100);
//		Invoke("StopIt",2f);
//		audio.loop = true; // Set the AudioClip to loop
		GetComponent<AudioSource>().mute = true; // Mute the sound, we don't want the player to hear it
//		while (!(Microphone.GetPosition(AudioInputDevice) > 0)){} // Wait until the recording has started
//		audio.mute = false;
	}

	void StopIt()
	{
		Debug.Log("stop");
		Microphone.End (null);
	}
	void OnGUI() 
	{
		if(GUI.Button(new Rect(10,70,60,50),"Record"))
		{ 
			Debug.Log("record");
			myClip = Microphone.Start(null, false, 3, 44100);
			Invoke("StopIt",3f);
		} 
		if (GUI.Button(new Rect(110,70,60,50),"Play"))
		{ 
			GetComponent<AudioSource>().clip = myClip;
			GetComponent<AudioSource>().Play ();
			GetComponent<AudioSource>().mute = false;
		} 

		GUI.Label(new Rect(100,200,100,100),"Vol = "+GetAveragedVolume());
		GUI.Label(new Rect(200,200,100,100),"Last Samples Vol = "+LastSamplesVolume());
	}

	/// <summary>
	/// Record this audio.
	/// </summary>
	public void Record()
	{
		GetComponent<AudioSource>().pitch = 1.0f;
		Debug.Log("record");
		GetComponent<AudioSource>().clip = Microphone.Start(null, false, 10, 44100);
		while (!(Microphone.GetPosition(null) > 0) ){}    //TODO: untoggle
		Debug.Log("while condition");
		GetComponent<AudioSource>().Play();
		GetComponent<AudioSource>().mute = true;
		StartCoroutine(CheckWhenToStop());
	}

	void CanStop()
	{
		StartCoroutine(CheckWhenToStop());
	}

	/// <summary>
	/// Checks  when the audio should stop.
	/// </summary>
	/// <returns>The when to stop.</returns>
	IEnumerator CheckWhenToStop()
	{
		float timer = 0.0f;
		float clipTime = 0.01f;
		int consecutiveNullVoice = 0;
		bool onceZero = false;
		while(consecutiveNullVoice < 10)
		{
			while( (LastSamplesVolume() > averageVolumeBuffer) )
			{
				while(timer < 0.1f)
				{
					timer+=0.02f;
					clipTime+=0.02f;
					yield return 0;
				}
				onceZero = false;
				timer = 0.0f;
				yield return 0;
				Debug.Log("funct");
			}
			if(onceZero)
			{
				consecutiveNullVoice++;
				timer = 0.0f;
				while(timer < 0.08f)
				{
					timer+=0.02f;
					clipTime+=0.02f;
					yield return 0;
				}
				timer = 0.0f;
			}
			else
				consecutiveNullVoice = 1;
			onceZero = true;
			Debug.Log("consecutiveNullVoice = "+consecutiveNullVoice);
			yield return 0;
		}
#if UNITY_EDITOR
		PlaySound (clipTime);
#else
		PlaySound (clipTime+0.3f);
#endif
	}


	/// <summary>
	/// Plaies the sound.
	/// </summary>
	/// <param name="clipTime">Clip time.</param>
	public void PlaySound(float clipTime)
	{
		GetComponent<AudioSource>().pitch = 1.0f;
		Microphone.End (null);
		GetComponent<AudioSource>().mute = false;
		Debug.Log("play");
//		audio.clip = myClip;
		GetComponent<AudioSource>().Play ();
		Debug.Log("Clip Length = "+clipTime);
		CancelInvoke("MuteIt");
		Invoke("MuteIt",clipTime);
	}
	
	/// <summary>
	/// Stops and mutes the audisource
	/// </summary>
	void MuteIt()
	{
		Debug.Log("mute");
		GetComponent<AudioSource>().Stop ();
		GetComponent<AudioSource>().mute = true;
	}

	/// <summary>
	/// Changes the data.
	/// </summary>
	void ChangeData()
	{
		float[] samples = new float[GetComponent<AudioSource>().clip.samples * GetComponent<AudioSource>().clip.channels];
		GetComponent<AudioSource>().clip.GetData(samples, 0);
		int i = 0;
		while (i < samples.Length) {
			samples[i] = samples[i] * 0.1F;
			++i;
		}
		GetComponent<AudioSource>().clip.SetData(samples, 0);
	}
	

	/// <summary>
	/// Volume of the last samples
	/// </summary>
	/// <returns>The last samples volume.</returns>
	float LastSamplesVolume()
	{ 
		float[] data = new float[256];
		float a = 0;
		GetComponent<AudioSource>().GetOutputData(data,0);

		for(int i = 240;i< 256;i++)
		{
			a += Mathf.Abs(data[i]);
		}
		lastVolume = a/16;
		return a/16;
	}

	/// <summary>
	/// Gets the averaged volume.
	/// </summary>
	/// <returns>The averaged volume.</returns>
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

	/// <summary>
	/// Analyzes the sound - find pitch
	/// </summary>
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



}
