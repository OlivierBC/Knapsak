# Knapsak
# Made by: Olivier Briand-Champagne

This is a project for a class centered around programming a website, following strict rules. It was a project made in a team of 4, using the Agile Scrum dev methods. Because of some unfortunate events, I have made the majority of this project myself (around 85%, more details later), while having no prior experience in fullstack web dev using c# and ASP.NET Core.The semester before the one this project was started in, I learned PHP and the basics of MVC environnements. So I had to learn a few things on the go.

It uses a deprecated version of ASP.NET Core and .Net 6.0, because the teacher/school offered a MySQL (mariaDB) database for free. It seemed it was only working on that version. It wasn't a great idea to go with that even though it was free. I should've used a non-deprecated version on a SQL server DB instead, it would've helped a lot especially with EntityFramework and the session variable. The problem with session variable made me create a model called SessionService and use TempData instead.
Another bad decision was to make every DB request in C# using LinQ instead of making them in the DB using functions and procedures instead.

Nonetheless I really enjoyed making this project, but I won't be coming back to it soon, because I have other intrests to explore in coding like game making, simulations, to learn a low-level language, get better in algorithms and understanding/perfecting memory usage... and more 

What did I do myself?
	As i said earlier, I made this project almost in its entirety.
	- 95% of the back-end
		- 100% of the DB-side
	- Set up Github
	- 80& of the JS
	- 90% of the HTML
What was done by the rest of the team?
	- Mostly the connexion and signing up system
	- HTML and CSS of some elements
	- And a lot of things that were changed :/
What was added after the end of the semester?
	- Bug fixes
	- Fixed the administration page to have all of the functionnalities that it was supposed to have using a lovely system of my own that uses a couple of partial views
What should be added next?
	- Autorefreshing
	- Portability (mobile app)
		- Would be easier to port using DB functions and procedures I think
	- Fixes for easier browsing on phone 

113/140 of the commits in the original git repo were made by me. I know the amount of commits in itself doesn't mean anything, but the original repo can't be made public, because the connection string was commit and pushed by someone.
