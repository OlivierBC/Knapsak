# Knapsak
#### Made by: Olivier Briand-Champagne

This project was created for a class focused on web development, following strict guidelines. It was originally developed by a team of four using Agile Scrum methodologies. Due to some unfortunate circumstances, I ended up completing the majority of the project myself (around 85%, with more details below), despite having no prior experience in full-stack web development using C# and ASP.NET Core. In the semester before this project began, I learned PHP and the basics of MVC environments, so I had to learn many concepts on the go.

The project uses a deprecated version of ASP.NET Core with .NET 6.0 because my school provided a free MySQL (MariaDB) database, which only worked reliably with that version. In hindsight, this was not an ideal choice. I could have used a free hosting solution or a local server for development and only deployed the project at the end of each Agile sprint for reviews, but I did not yet have that level of experience with web hosting when the project began. Using a non-deprecated ASP.NET version with a SQL Server database would have made development significantly easier, especially when working with Entity Framework and session variables. Issues with session variables led me to create a SessionService model and rely on TempData instead. Another questionable decision was handling all database queries in C# using LINQ rather than implementing database-side functions and stored procedures. That said, taking the longer and more difficult route seems to be a fairly natural instinct for developers, especially when the easier option exists 😅

Despite these shortcomings, I genuinely enjoyed working on this project. However, I do not plan to revisit it in the near future, as I want to explore other interests such as game development, simulations, learning a low-level language, improving my algorithmic skills, and deepening my understanding of memory usage.

### What I did myself:
I completed most of the project almost entirely on my own:

• ~95% of the back end

• 100% of the database layer

• GitHub setup

• ~80% of the JavaScript

• ~90% of the HTML

### What was done by the rest of the team:

• Most of the login and registration system

• HTML and CSS for some elements

• Several other features that were later modified or reworked

### What was added after the semester:

• Bug fixes

• A complete overhaul of the administration page to include all intended functionalities, implemented using a custom system based on multiple partial views

### What could be added next:

• Auto-refreshing features

• Improved portability (e.g., a mobile app)

• Easier portability through the use of database functions and stored procedures

• UI fixes for better mobile navigation

Out of 140 commits in the original Git repository, 113 were made by me. While commit count alone does not fully reflect contribution, the original repository cannot be made public because a connection string was accidentally committed and pushed by another team member.

