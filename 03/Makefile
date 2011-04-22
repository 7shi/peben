TARGET   = makeexe1.exe makeexe2.exe
CXX      = g++
CXXFLAGS = -std=c++0x

all: $(TARGET)

makeexe1.exe: makeexe1.cpp
	$(CXX) $(CXXFLAGS) -o $@ $<

makeexe2.exe: makeexe2.cpp
	$(CXX) $(CXXFLAGS) -o $@ $<

clean:
	rm -f $(TARGET) output.exe
