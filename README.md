# Задание 1
## Вариант 1

**Описание:** 
Требуется разработать компонент для анализа изображений с применением готовой нейронной сети в формате ONNX.  Приложение должно выдавать расстояние и меру схожести двух изображений

**Требования:**
1. Компонент должен создавать только один экземпляр модели (сессии), но предоставлять асинхронный потокобезопасный API.
2. Возможность отмены вычисления при помощи CancellationToken. 

### Запуск
Для запуска находясь в корневой директории выполните `dotnet run --project Application/Application.csproj`

Сравнение происходит между изображениями `face1.png` и `face2.png` из папки `img`.