all: main.cpp 
	gcc -fpic -shared main.cpp -o payload.so


