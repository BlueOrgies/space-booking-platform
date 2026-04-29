![BlueOrgies](BlueOrgies.webp)

# Space Booking Platform

A console-based booking platform for space travel, accommodation, and activities.

## Navigation

- [Project Description](#project-description)
- [Core Features](#core-features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [How To Run](#how-to-run)
- [Project Structure](#project-structure)
- [Coursework Documentation](#coursework-documentation)

## Project Description

Space Booking Platform allows users to discover and book sci-fi themed listings such as transport routes, accommodation, freight opportunities, and activities.

Users can register and log in as regular users or organizers.

- Regular users can browse listings, book, cancel bookings, and leave reviews.
- Organizers can create listings, edit their listings, cancel listings, and review booking outcomes.

The application is built as a terminal UI and uses SQLite for persistent storage.

## Core Features

- User registration and login
- Organizer mode for creating and managing listings
- Browse and search listings
- Booking and booking cancellation
- Listing cancellation by organizer
- Reviews on past bookings
- Organizer rating/feedback support
- Capacity handling with both seat/bed limits and weight-based freight limits

## Tech Stack

- Language: C#
- Runtime: .NET
- UI: Spectre.Console
- Database: SQLite

## Getting Started

### Prerequisites

- .NET SDK installed
- Git installed

### Clone

```bash
git clone https://github.com/BlueOrgies/space-booking-platform.git
cd space-booking-platform
```

## How To Run

From the repository root:

```bash
dotnet build
dotnet run --project space-booking-platform/space-booking-platform.csproj
```

## Project Structure

- `Models`: Domain models and enums
- `Views`: Spectre.Console UI screens
- `Services`: Business logic and database operations
- `Database.cs`: Database setup and table creation
- `Seeder.cs`: Initial seed data for users/listings/bookings/reviews
- `AppState.cs`: Session and navigation state
- `ViewHandler.cs`: View routing/navigation flow

---

## Coursework Documentation

- [User stories](#user-stories)
- [UML](#uml)
- [Process Report](#process-report)
- [Reflection](#reflection)
- [AI Usage](#ai-usage)

### User stories

- As a user, I want to be able to create a user and log in, to use the Application.
	- User can register and become a customer
- As a user, I want to be able to become an organizer, to create events
	- User(customer) can become an organizer from the main menu
- As an organizer, I want to create an event, so that customers can book them
	- Organizer can create an event
- As an organizer, I want to be able to change the details or cancel an event
	- Organizer can edit or cancel the event
	- Organizer cannot edit or delete expired events
- As a customer, I want to browse and search available events
	- Customer can browse available events
	- Customer can search in available events
- As a customer, I want to be able to book Space travel
	- Customer can book Space travel
	- Organizer cannot book their own event
- As a customer, I want to be able to leave a review after the trip or stay
	- Customer leaves review to the organizer
- As a customer I want to view my bookings and have the option to cancel
	- Customer can view bookings
	- Customer can choose to cancel booking
	- Customer cannot cancel booking x days before departure
	- Customer cannot cancel past bookings
- As an organizer I want to be able to view my past and upcoming events
	- Organizer can view past and upcoming events
	- Organizer can edit or cancel upcoming events

### UML

![User Booking Management UML](User%20Booking%20Management-2026-03-31-170729.svg)

### Project Board /Kanban
[GitHub Project Board](https://github.com/orgs/BlueOrgies/projects/1)
    

### Process Report

#### Philosophy

From the beginning we decided to use Scrum, and planned 3 sprints of 1 week each.
We stuck to this structure throughout the project, which helped knowing wich tasks should
be completed first and what the others were working on at the time. This worked well for us.

#### Roles

We rotated the role of Scrum master each week, and had a sprint review after each sprint.
This could have been done more structured, but we managed to go through everything and did a good
job delegating the different tasks. People completed the tasks they were given, and we did not
need to change anything during the sprints.

#### Development practices

We used Git for Version Control, and created a Kanban board with all the issues from the start.
After the sprint reviews we added issues that came up during the sprint, and created new issues from existing
ones which we needed to revise.

In Git, we created a dev branch from main, and then branched sprint1, sprint2 and sprint3 as they happened. Merging
into these branches required a review from another team member, which have worked out well. We
managed to set up some rules on the Project in GitHub, for example that you cannot push to main
without a review.

We met at the end of every sprint in person, and discussed and managed some issues together. During the sprints,
we communicated through Discord with questions that came up along the way.

#### Reflection

We have worked great together as a team and have delegated the tasks fairly. The code has come
together nicely, and we have taken ideas from each other to make it consistent.
There has been no issues with communication, and everyone has contributed. We have documented
along the way and dealt with issues as they arise. For next time, the sprint reviews and sprint retrospectives
could be more structured, but we still got through everything as planned.

##### Kristiane:

Planning and executing this project has been smooth, as it is similar in structure to our previous
arbeidskrav. It has been valuable to create something based on multiple peoples knowledge and opinions,
and I am very happy with the results. Working together in this team has been fun and stress-free. It is a
new experience working as a group of three, showing the importance of proper planning and prioritization.

##### Aleksander:

This is my first project where I have worked with clear architecture.
It was helpful to work with an inspired MVC structure as it made the project navigable and it was more comprehensible than mixing models and logic as I have done in previous projects.
In addition, this is my first time working with the Spectre framework and SQlite as a database.
I have learnt a lot from my team members as they introduced me to both Spectre and SQlite, which I applied when building Views or when working with the database.

Instead of having fixed roles we divided tasks based on the goals for that week.
I have worked with Services to handle database communication, Views for the terminal UI using Spectre,
ViewHandler to manage control flow between views, and AppState to maintain application state.

Working in a team of three has been a great experience and I believe we structured and planned this project well as it made progress consistent, and the outcome was predictable.
It has taught me the importance of planning out a project, and that splitting problems into smaller pieces is the key to make a project manageable.
As problems occurred along the way, we have been helpful to one another. Everybody has also had a say in the project's structure.

#### Thomas

Working on this project has been a fun. Coming from a frontend background, I am used to building multi-page applications quickly, and that likely influenced how I approached the structure of this project. In particular, I think that background shaped the decision to use AppState and ViewHandler, since I am already familiar with React and state-management tools such as Zustand.

I also think we worked very well together as a team. Communication throughout the sprints was easy, and meeting in person each week for sprint reviews and planning made it straightforward to discuss progress, solve issues, and coordinate the next steps.

The project did deviate somewhat from the original plan, as shown in the UML compared with the final result, but I think we handled those changes well. We continuously added new issues, discussed them during sprint reviews, and adjusted or planned features as needed. That made the development process flexible while still keeping the project moving forward.

### AI Usage
Prompts:
```
Please extend MapListings with the new and extended models.

```


```
Can you change CreateListingiew and EditListingView to work with the new models structure?

```


```
Please add the missing fields to the table in Database.cs

```


```
Please extend the data in seeder, take into account any new fields.

```


```
Can you create a README for this project? Make sure to not edit the Coursework Documentation section.
```
