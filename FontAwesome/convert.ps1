cd $PSScriptRoot

New-Item -ItemType Directory "icons\regular\" -ErrorAction SilentlyContinue
New-Item -ItemType Directory "icons\solid\" -ErrorAction SilentlyContinue

# fill first sets fill colour to white
# colorize sets all colours to fill colour

mogrify -verbose -resize 16x16 -gravity center -extent 16x16 -background none -fill `#c4c4c4 -colorize 100 -format png -path .\icons\regular\ .\svgs~\regular\*.svg
mogrify -verbose -resize 16x16 -gravity center -extent 16x16 -background none -fill `#c4c4c4 -colorize 100 -format png -path .\icons\solid\ .\svgs~\solid\*.svg