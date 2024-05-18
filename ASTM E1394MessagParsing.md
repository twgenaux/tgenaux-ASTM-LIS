---
title: ASTM E1394 Message Parsing
category: ASTM E1394
author: [ Theron W. Genaux, Revision 2, https://github.com/twgenaux/tgenaux-ASTM-LIS]
tags: [LIS,ASTM,E1394,LIS02]
---

### ASTM E1394 Message Parsing

This project demonstrates generically extracting the contents of ASTM  E1394 messages

The following output is from a program that was developed to explore two ideas; generically reading and writing ASTM messages, and using bi-directional maps to map database orders, patients, and results to create and read ASTM messages. The program supports round-tripping, such that the extracted content from one message can be used to recreate the equivelent original message. This program also works with HL7 Version 2.5 message files.


Input message:

> H|\^&|||OCD^VISION^5.10.0.46252^JNumber|||||||P|LIS2-A|20240307151237
> P|1|PID123456||NID123456^MID123456^OID123456|Brown^Bobby^B|White|19650102030400|U||||||||||||||||||||||||||
> O|1|SID101||ABO-D|N|20240307151207|||||||||CENTBLOOD|||||||20240307151237|||F|||||
> O|1|SID101||ABScr|N|20240307151207|||||||||CENTBLOOD|||||||20240307151237|||F|||||
> L||
>

Parse massage into record Position/Value pairss:

> Delimiters:|\^&
> H.1:H
> H.5:OCD^VISION^5.10.0.46252^JNumber
> H.5.1.1:OCD
> H.5.1.2:VISION
> H.5.1.3:5.10.0.46252
> H.5.1.4:JNumber
> H.12:P
> H.13:LIS2-A
> H.14:20240307151237
> P.1:P
> P.2:1
> P.3:PID123456
> P.5:NID123456^MID123456^OID123456
> P.5.1.1:NID123456
> P.5.1.2:MID123456
> P.5.1.3:OID123456
> P.6:Brown^Bobby^B
> P.6.1.1:Brown
> P.6.1.2:Bobby
> P.6.1.3:B
> P.7:White
> P.8:19650102030400
> P.9:U
> O.1:O
> O.2:1
> O.3:SID101
> O.5:ABO-D
> O.6:N
> O.7:20240307151207
> O.16:CENTBLOOD
> O.23:20240307151237
> O.26:F
> O.1:O
> O.2:1
> O.3:SID101
> O.5:ABScr
> O.6:N
> O.7:20240307151207
> O.16:CENTBLOOD
> O.23:20240307151237
> O.26:F
> L.1:L

Extract message content into Key/Value pairs:

> Delimiters:|\^&
> H-RecordType:H
> H-SenderName:OCD^VISION^5.10.0.46252^JNumber
> H-Company:OCD
> H-InstrumentModel:VISION
> H-Version:5.10.0.46252
> H-InstrumentID:JNumber
> H-ProcessingID:P
> H-VeresionNumber:LIS2-A
> H-TimeStamp:20240307151237
> P-RecordType:P
> P-SeqNumber:1
> P-PatientID:PID123456
> P-PatientID3:NID123456^MID123456^OID123456
> P-PatientName:Brown^Bobby^B
> P-PatientNameLast:Brown
> P-PatientNameFirst:Bobby
> P-PatientNameMiddle:B
> P-MothersMaiden:White
> P-BirthDate:19650102030400
> P-Sex:U
> O-RecordType:O
> O-SeqNumber:1
> O-SampleIDs:SID101
> O-Profiles:ABO-D
> O-Priority:N
> O-RequestedTimeStamp:20240307151207
> O-SampleTypes:CENTBLOOD
> O-ReportedTime:20240307151237
> O-ReportType:F
> O-RecordType:O
> O-SeqNumber:1
> O-SampleIDs:SID101
> O-Profiles:ABScr
> O-Priority:N
> O-RequestedTimeStamp:20240307151207
> O-SampleTypes:CENTBLOOD
> O-ReportedTime:20240307151237
> O-ReportType:F
> L-RecordType:L

Recreate the message by first converting Key/Value pairs into record Position/Value pairs:

> Delimiters:|\^&
> H.1:H
> H.5:OCD^VISION^5.10.0.46252^JNumber
> H.5.1.1:OCD
> H.5.1.2:VISION
> H.5.1.3:5.10.0.46252
> H.5.1.4:JNumber
> H.12:P
> H.13:LIS2-A
> H.14:20240307151237
> P.1:P
> P.2:1
> P.3:PID123456
> P.5:NID123456^MID123456^OID123456
> P.6:Brown^Bobby^B
> P.6.1.1:Brown
> P.6.1.2:Bobby
> P.6.1.3:B
> P.7:White
> P.8:19650102030400
> P.9:U
> O.1:O
> O.2:1
> O.3:SID101
> O.5:ABO-D
> O.6:N
> O.7:20240307151207
> O.16:CENTBLOOD
> O.23:20240307151237
> O.26:F
> O.1:O
> O.2:1
> O.3:SID101
> O.5:ABScr
> O.6:N
> O.7:20240307151207
> O.16:CENTBLOOD
> O.23:20240307151237
> O.26:F
> L.1:L

Export Position/Value pairs into an ASTM message:

> H|\^&|||OCD^VISION^5.10.0.46252^JNumber|||||||P|LIS2-A|20240307151237
> P|1|PID123456||NID123456^MID123456^OID123456|Brown^Bobby^B|White|19650102030400|U
> O|1|SID101||ABO-D|N|20240307151207|||||||||CENTBLOOD|||||||20240307151237|||F
> O|1|SID101||ABScr|N|20240307151207|||||||||CENTBLOOD|||||||20240307151237|||F
> L



