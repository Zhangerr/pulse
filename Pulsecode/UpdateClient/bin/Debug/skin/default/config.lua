setPulseColor(color4(0.7,0.7,0,0.5))
setMaskHolds(true)
setClearFrame(false)
setBottomSpace(true)
for i = 0, 7, 1 do
	if (i % 2 == 1) then
		c = color4(0.3, 0.6, 1, 1)
		setBarColor(color4(0.3,0.3,0.3,1), i)
		setLaneWidth(40, i)
	else 
		c = color4(1, 1, 1, 1)
		setBarColor(color4(0.0,0.0,0.0,1), i)
		setLaneWidth(32, i)
	end
	setLightColor(c, i)
	setKeyColor(c, i)
	setBurstColor(color4(1,1,1,1), i)	
end
setScoreLocation(location(25,716))
setScoreSize(size(35,35))
setAccuracyLocation(location(375,716))
setAccuracySize(size(35,35))
setTextOverlap(0)
setColorHolds(false)