## Server Input Bug

When Replicate() is called on a non-owned nob, the IReplicateData never sends.
On the host client, a log is printed when calling Replicate ("SENT") and when Replicate is actually called ("RECEIVED").

**Instructions:**
- Click the button
- Watch the logs

**Expected:** "RECEIVED" will match "SENT"  
**Actual:** "RECEIVED" is always default


## Observer Replicate Bug

Observers see an object way ahead of where it should be. Once it stops, it gets reconciled back into reality.

**Instructions:**
- Click the button
- Watch it move on the client

**Expected:** Movement looks like it does for the owner and server  
**Actual:** Movement is too far ahead and jumps all over the place
