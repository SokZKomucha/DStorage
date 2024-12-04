# DStorage
(In developement) file storage system on Discord. For scientific purposes only.

<br>

## Setup

This web application is comprised of two parts, client and server. Both of them are run separately; following sections will guide you through the setup.

### Requirements

In order to run this project, the following dependencies are required to be installed on your system:
- Node.js (v20.10.0 or newer)
- npm (10.2.3 or newer)
- .NET (8.0.100 or newer)

### Client setup

The following steps should be taken to set-up client side of this web application:

```bash
cd client # If not inside already
npm install
npm run dev 
```

This opens a dev server on port 5173. Alternatively, you may build the entire client side to raw HTML/CSS/JS using `npm run build` instead.

### Server setup

An `appsettins.json` file in `/server` is **necessary** to run the application. An `appsettings-example.json` file is provided, which you may use. `SQLiteConnectionString` may be left as is, but you'll need to set `DiscordToken` to your Discord bot's token. I'm not gonna write a tutorial how to get one, there're already tons of them on the internet. After completing that, rename the file to `appsettings.json`.

Aside from runtime (assuming already installed), dependencies (should install automatically) and `appsettings.json`, an SQLite database (`database.db` file) is required in `/server/Data` directory, or whatever you've specified in appsettings. An already prepared file `database-example.db` is provided, with all commands from `template.sql` executed; make sure to rename it to `database.db` before running the application. 

<!-- Server dependencies -->

<br>

## Legal

I, or any of this project's contributors, take no legal responsibility for any damage, harm or inappropriate actions done using this tool. Its purpose is to demonstrate that such actions are possible, but it should not be exploited. Doing so may lead to your Discord server being closed, bot being deleted, or even you account being terminated. **Use at your own risk!**  

<br>

## Todo
- Server-side authentication - basically done
- Client-side authentication
- Discord bot initialization
- Dashboard, file list
- File upload, both server and client
- File route on server