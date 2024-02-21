@echo off

rem Pergunta ao usuário se deseja construir a imagem Docker
set /p build_option=Buildar a imagem Docker? (s/n): 
set image=lucasfogliarini/boraapi:latest
echo.

rem Verifica a escolha do usuário
if /i "%build_option%"=="s" (
    echo Buildando a imagem Docker... 'docker build -t %image% .'
    docker build -t %image% .
    echo.
)
echo Iniciando o conteiner Docker...
docker stop BoraApi
docker rm BoraApi
docker run -d --name BoraApi -p 5000:8080 --env-file docker.env %image%

echo.
echo Conteiner Docker BoraApi iniciado! (image: %image%)
echo.

pause
