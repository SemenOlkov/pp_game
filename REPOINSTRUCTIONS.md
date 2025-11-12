Начальная настройка
1. Клонирование репозитория
git clone https://github.com/SemenOlkov/pp_game.git
cd pp_game
2. Настройка Git LFS (только один раз)
git lfs install
3. Открытие проекта в Unity
Запусти Unity Hub

Add → Select folder → выбери папку проекта

Unity автоматически восстановит все пакеты и сгенерирует недостающие файлы

Рабочий процесс
Что коммитить (обязательно):
# Основные файлы проекта:
git add Assets/
git add ProjectSettings/
git add Packages/manifest.json
git add UserSettings/

# Если созданы новые файлы:
git add .gitignore
git add README.md
Что НЕ коммитить (уже в .gitignore):
Library/ - генерируется автоматически

Temp/ - временные файлы

Logs/ - логи Unity

Build/ - папки сборки

*.csproj, *.sln - файлы IDE

.vs/ - настройки Visual Studio

Стандартные команды для работы:
# Перед началом работы
git pull origin master

# После внесения изменений
git add Assets/ ProjectSettings/ Packages/manifest.json
git commit -m "Описание изменений"
git push origin master

Важные моменты
При конфликтах:
Не коммить сгенерированные файлы из Library/

Если конфликт в Assets/ - нужно согласовать изменения

При конфликте в ProjectSettings/ - используй версию из последнего коммита

Перед пушингом:
git pull origin master
# Реши конфликты если есть
git push origin master
Если что-то пошло не так
Восстановление после ошибок:
bash
# Отменить локальные изменения
git checkout -- .

# Восстановить из последнего коммита
git reset --hard HEAD

# Полностью перезагрузить проект
git fetch origin
git reset --hard origin/master
Структура проекта
Assets/ - основные ресурсы проекта

ProjectSettings/ - настройки Unity

Packages/manifest.json - список пакетов

UserSettings/ - пользовательские настройки

Важно: Все бинарные файлы (текстуры, аудио, модели) автоматически обрабатываются через Git LFS