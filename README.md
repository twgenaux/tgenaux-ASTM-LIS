# ASTM E1394 and HL7 v2 Message Parsing and related Code and Projects


## ASTM E1394 Message Parsing
In this repository is code and programs related to ASTM E1394, ASTM E1381, LIS1, LIS2, and related code and projects.

This project demonstrates generically extracting the contents of ASTM  E1394 messages. 

In 2015 I started working with vendors and customers to help them interface with my company's new instrument. The message format was ASTM E1394 (ASTM). I started to wonder how LIS and middleware vendors were able to adapt to connecting with so many instruments where each instrument manufacturer developed their own format based on the ASTM E1394 standard.

I had already seen some code that reads and writes ASTM messages, both on the web and proprietary. And I always thought that they were error prone to use and and overly complex. I started  wondering how reading, writng, and processing ASTM messaging could be generalized so that one could adapt to all the variations that I've seen. I also wondered how to make it less error prone.

I played around mentally going through different ideas for a while until I realized that an ASTM record is in essence a recursive data structure, one recursion for each separator. ASTM records have 3 separators; Field, Repeat-Field, and Components. Because it only has 3 delimiters, it is limited to only 3 levels of recursion.

[ASTM E1394 Message Parsing](https://github.com/twgenaux/tgenaux-ASTM-LIS/ASTM E1394 Message Parsing.html) 

This project demonstrates generically extracting the contents of ASTM  E1394 messages.

