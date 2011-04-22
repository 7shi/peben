TARGET   = makeexe1.exe makeexe2.exe makeexe3.exe test1.o
AS       = as
CXX      = g++
CXXFLAGS = -std=c++0x

all: $(TARGET)

makeexe1.exe: makeexe1.cpp
	$(CXX) $(CXXFLAGS) -o $@ $<

makeexe2.exe: makeexe2.cpp
	$(CXX) $(CXXFLAGS) -o $@ $<

makeexe3.exe: makeexe3.cpp
	$(CXX) $(CXXFLAGS) -o $@ $<

test1.o: test1.s
	$(AS) -o $@ $<

clean:
	rm -f $(TARGET) output.exe
