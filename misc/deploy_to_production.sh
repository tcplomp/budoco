cd budoco
git pull
dotnet publish -o ../publish_next
# timestamp it
cd ..
touch publish_next/$( date '+%Y-%m-%d_%H-%M-%S' )
mv publish publish_prev
mv publish_next publish