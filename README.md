# Merging a Photo Image and Pen Input Image in Power Apps

this HTTP Trigger Functions can allow it!

# MergeImages
Merge two image and return one image as data uri.<br>
The two image is like a "image from camera control or image control" and "pen input contorol".<br>
if you use the function, you can make apps like draw on photo or image with powerapps.<br>

PowerApps→PowerAutomate→Call functionsAPI→Response to PowerApps

exsample
<image src="https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/293667/a02af594-b55e-a478-4b7d-4c54122887e1.gif" width="400px">

### Request

```
Content-type:application/json
{
    "image_fg_datauri":"data:image/png;base64,iVBORw0KGgoAAAmM～",
    "image_bg_datauri":"data:image/png;base64,iVBORw0KGgoAAAAN～",
    "crop_fg_image":"true",
}
```

### Response

```
Content-type:application/json
{
"data":"data:image/png～"
}
```
