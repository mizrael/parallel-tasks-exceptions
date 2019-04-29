# parallel-tasks-exceptions

Task.WaitAll() and Task.WhenAll() have a different way to handle exceptions:
Task.WaitAll() will collect the inner exceptions and wrap them in an AggregateException.
Task.WhenAll() instead is re-throwing only the first exception happening. 

The only way to retrieve the others is to not await directly the call to Task.WhenAll() but to store instead the returned Task 
in a variable. 
In the try/catch block then we can access the task.Exception property, which is an AggregateException, and do whatever we want 
with its InnerExceptions.

##

![alt text](https://raw.githubusercontent.com/mizrael/parallel-tasks-exceptions/master/screenshot.jpg "handling exceptions from Task.WaitAll and Task.WhenAll")
