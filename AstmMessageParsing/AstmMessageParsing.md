---
title: ASTM E1394 Message Parsing
category: ASTM E1394
author: [ Theron W. Genaux, Draft, 3-March-2024]
tags: [LIS,ASTM,E1394,LIS02]
---

Demonstrates generically extracting the contents of ASTM  E1394 messages. The contents can then be written directly to a database or can be converted to XML and json files.



> Creativity is not a talent; it is a way of operating, a mode of behaving. When I say "a way of operating," what I mean is this: Creativity is not an ability that you either have or do not have. It is, for example, and this may surprise you, absolutely unrelated to IQ, provided you're intelligent above a certain minimal level. In fact, the most creative people simply have acquired a facility for getting themselves into a particular mood-a way of operating-which allows their creative abilities to function. MacKinnon described the particular facility as "an ability to play." Indeed, he described the most creative people, when in this mood, as being childlike, for they were able to play with ideas, to explore them, not for any immediate practical purpose, but just for the enjoyment of playing, the delight of exploration.
>
> John Cleese



In 2015 I started working with vendors and customers to help them interface with my company's new instrument. The message format was ASTM E1394 (ASTM). I started to wonder how LIS and middleware vendors were able to adapt to connecting with so many instruments where each instrument manufacturer developed their own format based on the ASTM E1394 standard.

I have already seen some code that reads and writes ASTM messages, both on the web and proprietary. And I always thought that they were error prone to use and and overly complex. When working on updating some code, my unit testing showed that I had made an off-by-one error. I started to wonder how it could be generalized so that it could adapt to all the variations that I've seen. I also wondered how to make it kess error prone.



When I develope software, I try to design it so that other developers will find it be easy to use correctly, and hard to use incorrectly. 
