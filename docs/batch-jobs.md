# Batch Jobs

## Overview

Batch jobs allow you to process multiple media assets in parallel, applying the same or different properties to each
job. This is useful for tasks such as transcoding, analysis, or applying effects to multiple assets at once.

## Example

```json
{
  "JobName": "package-audio",
  "Jobs": [
    {
      "Inputs": [
        {
          "Name": "input-track",
          "Uri": "file://path/to/input/track.mp3"
        }
      ],
      "Outputs": [
        {
          "Name": "output-track",
          "Uri": "file://path/to/output/track.mp3"
        }
      ],
      "Properties": [
        {
          "Name": "audio-format",
          "Value": "mp3"
        },
        {
          "Name": "audio-bitrate",
          "Value": "128k"
        }
      ]
    },
    {
      "Inputs": [
        {
          "Name": "input-track",
          "Uri": "file://path/to/another/input/track.wav"
        }
      ],
      "Outputs": [
        {
          "Name": "output-track",
          "Uri": "file://path/to/another/output/track.m4a"
        }
      ],
      "Properties": [
        {
          "Name": "audio-format",
          "Value": "m4a"
        },
        {
          "Name": "audio-bitrate",
          "Value": "256k"
        }
      ]
    }
  ]
}
```