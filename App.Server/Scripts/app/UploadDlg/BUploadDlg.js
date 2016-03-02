'use strict';
function BUploadDlg()
{
    BusinessLayer.call(this);
    this.PublicForData = new BUploadDlg_ForData();
    this.PublicForView = new BUploadDlg_ForView();
    this.PublicForParent = new BUploadDlg_ForParent();

    var _files = null;
    var _completeDelegate = null;
    var _entityUsageId = null;
    var _entity = null;

    var ChunkSize = 100000;
    var RetryCount = 10;

    var me = this;

    this.GetData = function (postData)
    {
        postData.WebRequestCallback = 'DialogDataLoaded';
        postData.WebRequestUrl = getTemplateUrl;
        //postData.WaitingId = WaitersIds.DldWorkingWaiter;
        this.DataLayer.Request(postData);
    };

    this.ShowDialog = function (files, completeDelegate, entityUsageId, entity)
    {
        
        window.Waiting.AddIndividualWaiter( 'uploadWaiter', function () { }, function () { }, this);

        _entity = entity;
        _files = files;
        _completeDelegate = completeDelegate;
        _entityUsageId = entityUsageId;

        if (Metadata.APPLICATION$UploadPacketSize)
            ChunkSize = Metadata.APPLICATION$UploadPacketSize;



        window.VPopup.ShowDialog({
            templateId: HtmlTemplateIds.UploadDlgTemplate,
            model: this.ViewLayer,
            caption: GetTxt('Uploadng file'),
            Buttons: [DialogButtons.Cancel],
            width: 350,
            height: 250
        });


        


        for (var f in files)
        {
            currentFile = files[f];
            chunksCount = currentFile.size / ChunkSize;
            this.ViewLayer.FileChanged(currentFile.name, currentFile.size);
            
            reader = null;
            tryCount = 0;
            chunkNumber = 0;
            stopUpload = false;
            _uploadId = null;
            startRequest = null;

            Upload();
            break;
        }


    };

    var currentFile = null;
    var reader = null;
    var tryCount = 0;
    var chunkNumber = 0;
    var chunksCount = 0;
    var stopUpload = false;
    var _uploadId = null;
    var startRequest = null;

    function Upload(file)
    {
        if(!reader)
            reader = new FileReader();

        var start = ChunkSize * chunkNumber;
        var stop = start + ChunkSize;
        

        reader.onloadend = function (evt)
        {
            if (stopUpload)
            {
                return;
            }

            evt = evt || window.event;
            if (evt.target.readyState == FileReader.DONE)
            {
                if(evt.target.error)
                {
                    ShowFileReadError(evt);
                    return;
                }


                var postData = new PostDataObject();

                postData.WebRequestCallback = "ChunkUploadedResponse";
                postData.WebRequestUrl = uploadUrl;

                if (chunkNumber == 0) 
                {
                    postData.PostData.entityUsageId = _entityUsageId;
                    postData.PostData.attributeId = currentFile.AttrId;
                }

                var bytes = new Uint8Array(evt.target.result);
                var bytesRaw = [];
                for (var i = 0; i < bytes.byteLength; i++)
                {
                    bytesRaw.push(bytes[i]);
                }

                postData.PostData.dataStr = JSON.stringify( bytesRaw );
                postData.PostData.chunkNumber = chunkNumber;
                postData.PostData.fileLenght = currentFile.size;
                postData.PostData.uploadId = _uploadId;
                postData.WaitingId = "uploadWaiter";
                
                
                
                startRequest = new Date();

                me.DataLayer.Request(postData);

                
            }
            else
            {
                if(evt.target.error)
                {
                    ShowFileReadError(evt);
                    return;
                }
            }
        };

        if (currentFile.webkitSlice)
        {
            var blob = currentFile.webkitSlice(start, stop );
        } else if (currentFile.mozSlice)
        {
            var blob = currentFile.mozSlice(start, stop );
        } else if (currentFile.slice)
        {
            var blob = currentFile.slice(start, stop );
        }

        try
        {
            reader.readAsArrayBuffer(blob);
        }
        catch(er){}
        
}

    function CalculateSpeed()
    {
        var spendTicks = (new Date() - startRequest);
        return Math.floor( (ChunkSize * 1000 / spendTicks) / 1024 );
    };

this.ChunkUploadedResponse = function (data)
{
    if (stopUpload)
    {
        return;
    }

    if (chunkNumber < chunksCount && !stopUpload)
    {
        if (data.uploadId)
            _uploadId = data.uploadId;
        var bytesUploaded = chunkNumber * ChunkSize;
        this.ViewLayer.NextChunkUploaded( bytesUploaded, CalculateSpeed(), Math.round( bytesUploaded * 100 / currentFile.size ) );
        chunkNumber++;
        Upload();
    }
    else
    {
        _entity[currentFile.AttrId] = _uploadId; 
        delete _files[currentFile.AttrId];
        currentFile = null;

        tryCount = 0;
        chunkNumber = 0;
        chunksCount = 0;
        _uploadId = null;
        reader = null;

        var hasFiles = false;
        for (var f in _files)
        {
            hasFiles = true;
            currentFile = _files[f];
            chunksCount = currentFile.size / ChunkSize
            Upload();
            break;
        }

        if (!hasFiles)
        {
            if (this.ViewLayer.SelfCloseDelegate)
            {
                this.ViewLayer.SelfCloseDelegate.func.call(this.ViewLayer.SelfCloseDelegate.context);
            }

            
            _completeDelegate.Invoke();
            _completeDelegate = null;
        }

    }

    

};

this.StopUpload = function ()
{
    stopUpload = true;
};



function ShowFileReadError(e)
{
    e = e || window.event; // get window.event if e argument missing (in IE)  

    var errorText = '';
    switch(e.target.error.code) {

        case e.target.error.NOT_FOUND_ERR:

            errorText = GetTxt( 'File not found' );

            break;

        case e.target.error.NOT_READABLE_ERR:

            errorText = GetTxt( 'File not readable' );

            break;

        case e.target.error.ABORT_ERR:

            errorText = GetTxt( 'Read operation was aborted' );

            break; 

        case e.target.error.SECURITY_ERR:

            errorText = GetTxt( 'File is in a locked state' );

            break;

        case e.target.error.ENCODING_ERR:

            errorText = GetTxt( 'The file is too long to encode in a "data://" URL');

            break;

        default:

            errorText = GetTxt( 'File read error - ') + e.target.error.code;

    }       

    ShowMessage([errorText], 'Read error', DialogButtons.OK, NxMessageBoxIcon.Error);

};

}





//     function InitUploadData(
//      CxUploadData uploadData,
//      Stream fileStream,
//      int chunkNumber,
//      Guid uploadId)
//{
//      long pointer = GetPointerByChunkNumber(chunkNumber);

//    uploadData.Data = new byte[GetChunkSize
//      (pointer, chunkNumber, fileStream.Length, ChunkSize)];
//    fileStream.Position = (int) pointer;
//    fileStream.Read(uploadData.Data, 0, uploadData.Data.Length);
//    uploadData.ChunkNumber = chunkNumber;
//    uploadData.UploadId = uploadId;
//}

////----------------------------------------------------------------------------
///// <summary>
///// Calculates and return pointer for uploading data array using given chunk number.
///// </summary>
//    protected long GetPointerByChunkNumber(long chunkNumber)
//{
//      return ChunkSize * chunkNumber;
//}
////----------------------------------------------------------------------------
///// <summary>
///// Calculates and return chunk size.
///// </summary>
//    protected long GetChunkSize(long pointer, long chunkNumber, long fileLenght, int chunkSize)
//{
//      if (fileLenght < chunkSize)//Chunk size more than file lenght
//return fileLenght;

//if ((pointer + chunkSize) <= (fileLenght - 1))//current(full size) chunk
//    return chunkSize;

//return fileLenght - pointer;//last file chunk(file lenght more then chunk size)

//}





//}

extend(BUploadDlg, BusinessLayer);

function BUploadDlg_ForParent()
{
    this.ShowDialog = function () { };

};

function BUploadDlg_ForView()
{

    this.GetData = function (postData) { };
    this.StopUpload = function () { };

};

function BUploadDlg_ForData()
{

    this.DialogDataLoaded = function (data) { };
    this.ChunkUploadedResponse = function (data) { };
};