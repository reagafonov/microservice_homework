Feature: Получение видеопотока

    По запросу пользователя ему передается видеопоток с камеры, в видоскателе которой находится машина
    Как водитель машины
    Чтобы быть в курсе того, что с ней происходит
    Я хочу иметь возможность по требованию получать видеопоток с камеры, наблюдающей машину
    Сценарий 1: При запросе видео я должен получить видеопоток
    Есть машина в поле зрения видеокамеры
    и я запрашиваю видеопоток
    тогда я получаю видеопоток 
    и в видеопотоке указаны
    1) изображение машины
    2) текущую дату
    3) место установки видеокамеры
    4) текущий статус событий
