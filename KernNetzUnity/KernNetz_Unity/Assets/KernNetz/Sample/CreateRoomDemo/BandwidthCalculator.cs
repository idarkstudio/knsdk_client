using FigNet.KernNetz;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BandwidthCalculator : MonoBehaviour
{
    private int initialBytesSent = 0;
    private int initialBytesReceived = 0;
    private Stopwatch stopwatch;
    public static float kiloBytesSentPerSec = 0f;
    public static float kiloBytesReceivedPerSec = 0f;
    void Start()
    {
        // Store the initial value of total bytes sent and received
        initialBytesSent =0;
        initialBytesReceived = 0;
        stopwatch = new Stopwatch();
        stopwatch.Start();
        // Start the coroutine to calculate the bandwidth rate
        StartCoroutine(CalculateBandwidthRate());
    }

    private IEnumerator CalculateBandwidthRate()
    {
        while (true)
        {
            if (KN.Socket == null) continue;
            // Record the current value of packets sent count and total bytes sent
            int currentBytesSent = (int)KN.Socket.NetStatistics.BytesSent;
            int currentBytesReceived = (int)KN.Socket.NetStatistics.BytesReceived;

            // Calculate the difference between the current and initial values of total bytes sent and received
            int bytesSentDifference = currentBytesSent - initialBytesSent;
            int bytesReceivedDifference = currentBytesReceived - initialBytesReceived;

            // Calculate the time elapsed since the last measurement
            float elapsedTime = stopwatch.ElapsedMilliseconds / 1000.0f;

            // Calculate the kilobytes per second rates
            kiloBytesSentPerSec = bytesSentDifference / 1024.0f / elapsedTime;
            kiloBytesReceivedPerSec = bytesReceivedDifference / 1024.0f / elapsedTime;

            // Reset the initial values to the current values for the next measurement
            initialBytesSent = currentBytesSent;
            initialBytesReceived = currentBytesReceived;

            // Wait for one second before taking the next measurement
            yield return new WaitForSeconds(1f);
        }
    }
}
