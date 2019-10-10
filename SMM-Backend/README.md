# SMM-Backend

This is the backend for ScoreManager and BTLD. In the following contents, I will describe each modules' function.

This backend uses PHP. You should configure correct PHP environment and set up Nginx or other Internet server well.

This backend needs HTTPS protocol to ensure each data is safe.

## Needed PHP plugins

* PDO
* PDO_Mysql

## Needed software

* MySql

## Nginx config

`config.php`, `utilities.php`, `preconfig.php` and `database.php` shouldn't be visited outside from the server, use following Nginx config to ban related connections.

```
location ~* ^/(preconfig|config|utilities|database).php {
	return 404;
}
```

After invoking `init.php`, you also can add `init` into above code to avoid more attack.

TIP: If you have invoked init.php, you also can add init.php into banned urls for preventing protential attack.

## API list

Go to [API document](./API.md).

## Database structure

Go to [Database document](./DATABASE.md).
