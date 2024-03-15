---
title: ASTM E1394 Message Parsing
category: ASTM E1394
author: [ Theron W. Genaux, Draft, 13-March-2024]
tags: [LIS,ASTM,E1394,LIS02]
---

Demonstrates generically extracting the contents of ASTM  E1394 messages. The contents can then be written directly to a database or converted to XML and json files. The code was designed to support round-tripping, such that the extracted content can be used to recreate the original message.

This project is the result of exploring two goals; generically reading and writing ASTM messages and two, using bi-directional maps to map database orders, patients, and results for the creation and reading ASTM messages. 

This code also works with HL7 Version 2.5 message files.

In 2015 I started working with vendors and customers to help them interface with my company's new instrument. The message format was ASTM E1394 (ASTM). I started to wonder how LIS and middleware vendors were able to adapt to connecting with so many instruments where each instrument manufacturer developed their own format based on the ASTM E1394 standard.

I had already seen some code that reads and writes ASTM messages, both on the web and proprietary. And I always thought that they were error prone to use and and overly complex. When working on adding to such code, my unit testing showed that I made an off-by-one error. I started to wonder how reading, writng, and processing ASTM messaging could be generalized so that one could adapt to all the variations that I've seen. I also wondered how to make it less error prone.

I played around with this for a while until I realized that an ASTM record is a recursive data structure. It has 3 delimiters, Field, Repeat-Field, and Components. Because it only has 3 delimiters, recursion is limited to only 3 levels.

> Creativity is not a talent; it is a way of operating, a mode of behaving. When I say "a way of operating," what I mean is this: Creativity is not an ability that you either have or do not have. It is, for example, and this may surprise you, absolutely unrelated to IQ, provided you're intelligent above a certain minimal level. In fact, the most creative people simply have acquired a facility for getting themselves into a particular mood-a way of operating-which allows their creative abilities to function. MacKinnon described the particular facility as "an ability to play." Indeed, he described the most creative people, when in this mood, as being childlike, for they were able <u>to play with ideas, to explore them, not for any immediate practical purpose, but just for the enjoyment of playing, the delight of exploration</u>.
>
> John Cleese

When I develop software, I try to design it so that other developers will find it be easy to use correctly, and hard to use incorrectly. 

