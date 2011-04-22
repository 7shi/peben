TARGET   = makeexe1.exe makeexe2.exe makeexe3.exe makeexe4.exe \
           test1.o prtnum.dll test2.exe test3.exe
AS       = as
CC       = gcc
CXX      = g++
CXXFLAGS = -std=c++0x

all: $(TARGET)

makeexe1.exe: makeexe1.cpp
	$(CXX) $(CXXFLAGS) -o $@ $<

makeexe2.exe: makeexe2.cpp
	$(CXX) $(CXXFLAGS) -o $@ $<

makeexe3.exe: makeexe3.cpp
	$(CXX) $(CXXFLAGS) -o $@ $<

makeexe4.exe: makeexe4.cpp
	$(CXX) $(CXXFLAGS) -o $@ $<

test1.o: test1.s
	$(AS) -o $@ $<

prtnum.dll: prtnum.c
	$(CC) -shared -o $@ $<

test2.exe: test2.c prtnum.dll
	$(CC) -o $@ $^

test3.exe: test3.c prtnum.dll
	$(CC) -o $@ -nostdlib $^

clean:
	rm -f $(TARGET) output.exe
