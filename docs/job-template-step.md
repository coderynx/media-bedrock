# Job Template Step

## Introduction

A job step template is used to define a single step in a job template.

## Properties

| Property Name | Type   | Description                                                              |
|---------------|--------|--------------------------------------------------------------------------|
| Name          | string | The name of the job step.                                                |
| DisplayName   | string | The display name of the job step.                                        |
| Description   | string | A description of the job step.                                           |
| ProcessorName | string | The name of the processor that will be used to execute the job step.     |
| Sinks         | array  | An array of sink objects that define the output of the job step.         |
| Sources       | array  | An array of source objects that define the input to the job step.        |
| Properties    | array  | An array of property objects that define the properties of the job step. |

## Example

The JSON below represents a step that encodes a Dolby Atmos mezzanine audio track to Dolby Digital Plus with JOC.

```json
{
  "Name": "encode-to-ddp",
  "DisplayName": "Encode Dolby Digital Plus with JOC",
  "Description": "Encode the Dolby Atmos mezzanine audio track to Dolby Digital Plus with JOC",
  "ProcessorName": "dolby/ddp-encoder",
  "Sinks": [
    {
      "Name": "input",
      "Source": "input-track"
    }
  ],
  "Sources": [
    {
      "Name": "output",
      "Destination": "encoded-track"
    }
  ],
  "Properties": [
    {
      "Name": "LineDrcProfile",
      "DisplayName": "Line DRC Profile",
      "Description": "The Line DRC profile to use",
      "Value": "${LineDrcProfile}"
    },
    {
      "Name": "RightLeftDrcProfile",
      "DisplayName": "Right/Left DRC Profile",
      "Description": "The Right/Left DRC profile to use",
      "Value": "${RightLeftDrcProfile}"
    }
  ]
}
```

## Sink

A sink is an input to the job step. It can be an audio track, a video track, or any other type of input that is required
by the job step.

### Properties

| Property Name | Type   | Description                                                        |
|---------------|--------|--------------------------------------------------------------------|
| Name          | string | The name of the sink.                                              |
| Source        | string | The name of the source that will be used as input to the job step. |

### Example

```json
{
  "Name": "input",
  "Source": "input-track"
}
```

## Source

A source is an output of the job step. It can be an audio track, a video track, or any other type of output that is
required
by the job step.

### Properties

| Property Name | Type   | Description                                                                |
|---------------|--------|----------------------------------------------------------------------------|
| Name          | string | The name of the source.                                                    |
| Destination   | string | The name of the destination that will be used as output from the job step. |

### Example

```json
{
  "Name": "output",
  "Destination": "encoded-track"
}
```

## Property

A property is a key-value pair that defines a property of the job step. It can be used to pass configuration values to
the
job step or to define other properties that are required by the job step.

### Properties

| Property Name | Type   | Description                       |
|---------------|--------|-----------------------------------|
| Name          | string | The name of the property.         |
| DisplayName   | string | The display name of the property. |
| Description   | string | A description of the property.    |
| Value         | string | The value of the property.        |

### Example

```json
{
  "Name": "LineDrcProfile",
  "DisplayName": "Line DRC Profile",
  "Description": "The Line DRC profile to use",
  "Value": "${LineDrcProfile}"
}
```