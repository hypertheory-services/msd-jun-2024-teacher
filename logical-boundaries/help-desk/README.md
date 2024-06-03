# The Help Desk Bounded Context

## The IssueTrackerSolution

Create a couple user jwts:

In the directory for the IssueTracker.Api, run this:

```powershell
dotnet user-jwts create -n sue@aol.com --role SoftwareCenter
```

```powershell
dotnet user-jwts create -n bob@aol.com
```

Copy the JWT into the
