


# Pragmatic Test-Driven Development in C# .NET 

<a href="https://www.packtpub.com/product/pragmatic-test-driven-development-in-c-net/9781803230191?utm_source=github&utm_medium=repository&utm_campaign="><img src="https://static.packt-cdn.com/products/9781803230191/cover/smaller" alt="Pragmatic Test-Driven Development in C# .NET " height="256px" align="right"></a>

This is the code repository for [Pragmatic Test-Driven Development in C# .NET ](https://www.packtpub.com/product/pragmatic-test-driven-development-in-c-net/9781803230191?utm_source=github&utm_medium=repository&utm_campaign=), published by Packt.

**Write loosely coupled, documented, and high-quality code with DDD using familiar tools and libraries**

## What is this book about?
his book takes you from little to no TDD or unit testing knowledge to implementing real-life projects based on .NET frameworks, such as ASP.NET Core and Entity Framework. You’ll also get to grips with software practices like DDD and SOLID along with learning how to champion the transformation in your team and educate the business on the benefits.

This book covers the following exciting features:
* Writing unit tests with xUnit and getting to grips with dependency injection
* Implementing test doubles and mocking with NSubstitute
* Using the TDD style for unit testing in conjunction with DDD and best practices
* Mixing TDD with the ASP.NET API, Entity Framework, and databases
* Moving to the next level by exploring continuous integration with GitHub
* Getting introduced to advanced mocking scenarios
* Championing your team and company for introducing TDD and unit testing

If you feel this book is for you, get your [copy](https://www.amazon.com/dp/803230193) today!

<a href="https://www.packtpub.com/?utm_source=github&utm_medium=banner&utm_campaign=GitHubBanner"><img src="https://raw.githubusercontent.com/PacktPublishing/GitHub/master/GitHub.png" 
alt="https://www.packtpub.com/" border="5" /></a>

## Instructions and Navigations
All of the code is organized into folders. For example, Chapter02.

The code will look like the following:
```
public class SampleTests
{
    private static int _staticField = 0;
    [Fact]
    public void UnitTest1()
      {
        _staticField += 1;
        Assert.Equal(1, _staticField);
      }
    [Fact]
    public void UnitTest2()
    {
      _staticField += 5;
      Assert.Equal(6, _staticField);
    }
}
```

**Following is what you need for this book:**
This book is for mid to senior-level .NET developers looking to use the potential of TDD to develop high-quality software. Basic knowledge of OOP and C# programming concepts is assumed but no knowledge of TDD or unit testing is expected. The book provides in-depth coverage of all the concepts of TDD and unit testing, making it an excellent guide for developers who want to build a TDD-based application from scratch or planning to introduce unit testing into their organization.

With the following software and hardware list you can run all code files present in the book (Chapter 1-15).
### Software and Hardware List
| Chapter | Software required | OS required |
| -------- | ------------------------------------ | ----------------------------------- |
| 1 | Visual Studio 2022 | Windows, Mac OS X, and Linux (Any) |
| 4 | Fine Code Coverage | Windows |
| 9 | SQL Server | Windows, Mac OS X, and Linux (Any) |
| 9 | Cosmos DB | Windows, Mac OS X, and Linux (Any) |


We also provide a PDF file that has color images of the screenshots/diagrams used in this book. [Click here to download it](https://packt.link/OzRlM).

### Related products
* Parallel Programming and Concurrency with C# 10 and .NET 6  [[Packt]](https://www.packtpub.com/product/parallel-programming-and-concurrency-with-c-10-and-net6/9781803243672?utm_source=github&utm_medium=repository&utm_campaign=) [[Amazon]](https://www.amazon.com/dp/1803243678)

* High-Performance Programming in C# and .NET  [[Packt]](https://www.packtpub.com/product/high-performance-programming-in-c-and-net/9781800564718?utm_source=github&utm_medium=repository&utm_campaign=) [[Amazon]](https://www.amazon.com/dp/1800564716)



## Get to Know the Author
**Adam Tibi**
is a London-based software consultant with over 22 years of experience in .NET, Python, the
Microsoft stack, and Azure. He is experienced in mentoring teams, designing architecture, promoting
agile and good software practices, and, of course, writing code. Adam has consulted for blue-chip firms
including Shell, Lloyds Bank, Lloyd’s of London, Willis Towers Watson, and for a mix of start-ups. As
a consultant who has a heterogeneous portfolio of clients, he has gained a solid understanding of the
TDD intricacies, which he has transferred into this book.

### Download a free PDF

 <i>If you have already purchased a print or Kindle version of this book, you can get a DRM-free PDF version at no cost.<br>Simply click on the link to claim your free PDF.</i>
<p align="center"> <a href="https://packt.link/free-ebook/9781803230191">https://packt.link/free-ebook/9781803230191 </a> </p>