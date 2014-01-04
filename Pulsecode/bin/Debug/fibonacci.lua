a=0 --first number
b=1 --second number
c=0 --counter
while c < 20 do --print the first 20 numbers of the sequence
	temp = a+b --add up the two numbers are store it
	b = a --assign the value of the first number to the second
	a = temp --assign added up to first
	print(a)
	c = c +1
end
