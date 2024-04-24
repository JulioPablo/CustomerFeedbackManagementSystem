# Assumptions

Code First Used in Entity Framework

Will not use DTOs for simplicity sake (for the most part)

Used very basic, dummy auth, no email confirmation or other advanced features.

Added the FeedbackReveicerEntity, which is supposed to be either a product, company or business that's getting the feedback

Went with a classic MVC using dedicated Controllers approach instead of using Razor Pages

Used https://github.com/ligershark/WebOptimizer for minification, can also be used for bundling but was not needed for this project

Did not include unit tests or integration tests
Used CRSF Token Validation

Used EF Core and never used unsanitized user input as a way to prevent SQL Injections

A user account must be created by registering to the app

Edit/Delete of feedback is only available to the respective user that created it, so there's a very basic level of authorization implemented

Target Framework can be .NET 5 or .NET 8


# How to run
Setup Instructions, open in Visual Studio
Open the Package Manager Console
Run 'dot
Run 'Update-Database'
