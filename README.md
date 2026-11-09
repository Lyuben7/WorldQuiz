# QuizLab

QuizLab е учебен сайт с куизи по география, HTML, C#, Java, ASP.NET, компютърни архитектури, компютърни системи и бази данни.

1. Стартирай MySQL от XAMPP.
2. В phpMyAdmin импортирай `database/quizlab.sql`.
3. От корена стартирай `dotnet run --project QuizLab`.

JSON източникът е `QuizLab/Data/quizzes.json`; генерирай SQL от него с `python database/export_json.py`.

Създай профил от „Регистрация“, за да се записват резултатите. След вход отвори „Постижения“ за историята на опитите и обобщения напредък. Профилите са в таблицата `users`, а резултатите — в `quiz_attempts`; паролите се съхраняват като хешове.
