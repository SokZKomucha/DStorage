# DStorage
(In developement) file storage on Discord. For scientific purposes only.

<br>

## Setup

This web application is comprised of two parts: client and server. Both of them are run separately; following sections will guide you through its setup.

### Requirements

In order to run this project, the following dependencies are required to be installed on your system:
- Node.js (v20.10.0 or newer)
- npm (10.2.3 or newer)
- .NET (8.0.100 or newer)

### Client setup

The following steps should be taken to set-up client side of this web application:

```bash
cd client # If not inside already
npm i
npm run dev 
```

This opens the dev server on port 5173. Alternatively, you may build the entire client side to raw HTML/CSS/JS using `npm run build` instead.

### Server setup

An SQLite database (database.db) file is required in `/server/Data/` directory. Typically you'd want to create it using [EF Core migrations](https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli), but in case of this application, I'd advise you to manually create `database.db` inside `/server/Data`. Using some kind of SQLite editor, run setup script from `/server/Data/template.sql` and save changes.

<br>

## Legal

I, or any of this project's contributors, take no legal responsibility for any damage, harm or inappropriate actions done using this tool. Its purpose is to demonstrate such thing is possible, but it should not be exploited. Doing so may lead to your Discord server being closed, bot being deleted, or even you account being terminated. **Use at your own risk!**  