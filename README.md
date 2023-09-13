## Server Input Bug
[GitHub Issue #460](https://github.com/firstGearGames/fishNet/issues/460)

When Replicate() is called by the server on an ownerless nob, the IReplicateData gets lost.
To show the issue, a log is printed when calling Replicate ("SENT") and when Replicate is actually called ("RECEIVED") on the server.

**Instructions:**
- Click the button
- Watch the logs

**Expected:** "RECEIVED" will match "SENT"  
**Actual:** "RECEIVED" is always default

https://github.com/RyanGarber/FishNet-PV2/assets/2010001/de5454c4-96c0-476c-8f44-f489df9d7c33

---

## Observer Replicate Bug
[GitHub Issue #461](https://github.com/firstGearGames/fishNet/issues/461)

Observers see an object way ahead of where it should be. Once it stops, it gets reconciled back into reality. It also looks jumps around and looks choppy.

**Instructions:**
- Click the button
- Watch it move on the client

**Expected:** Movement looks like it does for the owner and server  
**Actual:** Movement is too far ahead and jumps all over the place

https://github.com/RyanGarber/FishNet-PV2/assets/2010001/0b80da39-4c3f-4b16-8503-8c2ab216c31c
