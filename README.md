# Intellias Hackaton

## Summary

Solution uses mongoDB as a persistence. Data schema probably is far from optimal. Probably if more denornalized schema would've been chosen, the solutiond could look better and do not contain such unmaintainable mongo queries.
Also I suppose it could be interesting to implement this task with some graph db, but unfortunately i do not have experience of working with them.    

## Run

```bash
# Go to the root repo directory run:
$ docker-compose up --build
```

## Test

_Example_

```bash
# Go to the root repo directory run:
$ dotnet test AssignmentService.sln
```

## Notes

<Put here your notes if you have some>
