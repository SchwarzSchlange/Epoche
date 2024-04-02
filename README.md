# Epoche

## A scripting language

### Features so far
1. Conditions (is,not)
2. Loops (until)
3. Echo (print,print_line)
4. Variable Defining (var,in)
5. Functions (block,call)
6. Other (break,clear)


### Features i want to add

1. Math expressions



### Example Code

```
program main

block sayHelloTo(name,times)
{
    until i:1==>@times
    {
        print 'Welcome @name'
    } 
}

in time

call sayHelloTo('admin',@time)
```


```
program main

var int a 30
var int b 30

is '@a' == '@b'
{
    print 'True!'
}
not
{
    print 'False!'
}
```

```
program main

until i:1==>30
{
    is '@i' == '10'
    {
        print 'Breaking!'
        break
    }
    not
    {
        print '@i'
    }
}
```


## There are many basic programs that can be done with Epoche Scripting Language
