# SUSI API @ http://susi.apphb.com
Ladies and gentleman, the moment you've all been waiting for - an API to our beloved SUSI

## How does it work?
It's a simple RESTful web service that works on two steps:

1. Send a `POST` request to `susi.apphb.com/api/login` containing the `username` and the `password` of your user. The server
will then return a key (GUID) that you can use to reference the user later.

2. Send another `POST` request containing your key (either to `susi.apphb.com/api/student` or `susi.apphb.com/api/courses`) to 
get whatever info you need (`/api/courses` requires an extra parameter, scroll down to the example section).

When done with your user, send a `DELETE` request to `susi.apphb.com/api/login` to dispose your key and log the user off.

## Notes
* The service accepts/returns data both as JSON and XML. Make sure you set the `Content-type` header to your prefered data format. (Haven't really tested the XML part, it may have bugs)
* If the student has dropped, both his year and his group will be set to 0.
* If a course has yet to be taken, its grade will be set to 0.
* There's a log file that logs some information about logging in and out and requesting information at [http://susi.apphb.com/log.html](http://susi.apphb.com/log.html)
* There are a number of different errors that may arise when using the service. Check out [error section](README.md#errors)
* The service returns the structures [StudentInfo](http://github.com/NikolaDimitroff/SusiParser/blob/master/SusiParser/StudentInfo.cs)/[CourseInfo](http://github.com/NikolaDimitroff/SusiParser/blob/master/SusiParser/CourseInfo.cs) serialized as JSON/XML. Check out [example response section](README.md#examples) for more information.

## Errors
### /api/login
Post request errors:
* 400 (Bad Request) - your credentials are invalid
* 502 (Bad Gateway) - the service failed to reach SUSI
* 503 (Service Unavailable) - reserved. Should never be returned, but is there to distinguish a random Internal Server Error (500) (which IIS returns by default on exception) from a business logic error
Delete request errors:
* 404 (Not Found) - key has expired / doesn't exist.

### /api/student
* 401 (Unauthorized) - key has expired / doesn't exist.

NOTE: Whenever the number of keys exceed 100 000, the last 10 000 are disposed. Due to that, there's a small chance that your key may be invalidated. If everyone disposes their keys (sends a `DELETE` request, see [How does it work](README.md#how-does-it-work) section) after they are done using them, no such thing will happen.
* 502 (Bad Gateway) - the service failed to reach SUSI

### /api/courses
* 401 & 502 - as above 
* 400 (Bad Request) - the coursesType parameter is missing or is not in the interval [0, 2]

## Examples
Here's a .NET Console app [here](http://github.com/NikolaDimitroff/SusiParser/blob/master/SusiParser.Console/ConsoleApp.cs) for you to play with that demonstrates working with the service.

And here are a few sample requests/responses:
### /api/login
POST - Request

	POST /api/login HTTP/1.1
	Host: susi.apphb.com
	Content-Type: application/json
	Content-Length: 46
	{ username: "username", password: "password" }
	
POST - Response

	HTTP/1.1 200 OK
	Content-Type: text/json;charset=utf-8
	Content-Length: 38
	"c78abca3-5fa4-41cd-967b-0022281d806b"
	
DELETE - Request

	DELETE /api/login HTTP/1.1
	Host: susi.apphb.com
	Content-Type: application/json
	Content-Length: 38
	"c78abca3-5fa4-41cd-967b-0022281d806b"
	
DELETE - Response

	HTTP/1.1 200 OK
	Content-Type: text/json;charset=utf-8
	Content-Length: 0
	
### /api/student
Request

	POST /api/student HTTP/1.1
	Host: susi.apphb.com
	Content-Type: application/json
	Content-Length: 38
	"c78abca3-5fa4-41cd-967b-0022281d806b"
	
Response

	HTTP/1.1 200 OK
	Content-Type: text/json;charset=utf-8
	Content-Length: 141	
	{"firstName":"Никола","middleName":"Димитров","lastName":"Димитров","facultyNumber":"61560","programme":"СИ(рб)","type":0,"year":2,"group":1}

### /api/courses	

There's an extra parameter here - coursesType. It is a number in the interval [0, 2] and is equivalent to the [CourseTakenType enumeration](https://github.com/NikolaDimitroff/SusiParser/blob/master/SusiParser/CourseTakenType.cs)
The parameter should be send in the URL i.e. `api/courses?coursesType=0`.

Parameter values / meanings:

0					Return all courses

1					Return only taken courses

2					Return only not taken courses

Request

	POST /api/student?coursesType=1 HTTP/1.1
	Host: susi.apphb.com
	Content-Type: application/json
	Content-Length: 38	
	"c78abca3-5fa4-41cd-967b-0022281d806b"
	
Response

	HTTP/1.1 200 OK
	Content-Type: text/json;charset=utf-8
	Content-Length: 1540
	[{"name":"Алгебра","teacher":"доц. д-р Пламен Николов Сидеров","grade":6.0,"isTaken":true,"isElective":false,"credits":7.0},{"name":"Анализ 1","teacher":"доц. д-р Владимир Димитров Бабев","grade":5.0,"isTaken":true,"isElective":false,"credits":7.0},{"name":"Английски език","teacher":"Десислава Иванова Гушева","grade":6.0,"isTaken":true,"isElective":false,"credits":2.0},{"name":"Дискретни структури 1","teacher":"доц. д-р Ангел Василев Дичев","grade":5.0,"isTaken":true,"isElective":false,"credits":7.0},{"name":"Увод в програмирането","teacher":"доц. д-р Александър Тодоров Григоров","grade":6.0,"isTaken":true,"isElective":false,"credits":5.5},{"name":"Анализ 2","teacher":"доц. д-р Владимир Димитров Бабев","grade":5.0,"isTaken":true,"isElective":false,"credits":7.0},{"name":"Геометрия","teacher":"доц. д-р Симеон Петров Замковой","grade":6.0,"isTaken":true,"isElective":false,"credits":7.0},{"name":"Дискретни структури 2","teacher":"доц. д-р Ангел Василев Дичев","grade":5.0,"isTaken":true,"isElective":false,"credits":6.0},{"name":"Компютърен английски език","teacher":"Десислава Иванова Гушева","grade":6.0,"isTaken":true,"isElective":false,"credits":2.0},{"name":"Обектно-ориентирано програмиране","teacher":"доц. д-р Александър Тодоров Григоров","grade":6.0,"isTaken":true,"isElective":false,"credits":5.5},{"name":"Спорт","teacher":"Липсва","grade":6.0,"isTaken":true,"isElective":false,"credits":4.0}]
	
## Contacts
Contact me at [nikola@dimitroff.bg](mailto:nikola@dimitroff.bg) or in facebook for suggestions and bugs.