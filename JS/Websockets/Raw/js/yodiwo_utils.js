// ------------------------------------------------------------------------------------
// Yodiwo JS utils Functions
// ------------------------------------------------------------------------------------

// UtilEnv and init function
UtilEnv = {
    containers:
		{
		    toolboxTooltip: null,
		    userTraceButton: null,
		},
    toolTipInterval: null
}

function initUtil() {
    // store useful containers for tooltip rendering
    UtilEnv.containers.toolboxTooltip = $('#cyan-tooltip');
    UtilEnv.containers.toolboxTooltipArrow = $('#cyan-tooltip .cyan-tooltip-arrow');
    UtilEnv.containers.toolboxTooltipText = $('#cyan-tooltip .cyan-tooltip-inner');
    // store container for userTraceButton
    UtilEnv.containers.userTraceButton = $('#userTraceButton');
    // assign handler for userTraceButton -> show myWebConsole modal
    UtilEnv.containers.userTraceButton.on('click', function () { $('#myWebConsole').modal('show'); });
}

// ------------------------------------------------------------------------------------
// Sidebars
// ------------------------------------------------------------------------------------

function openControlSidebar() {
    // check if sidebar is close and open
    if (!$('.control-sidebar').hasClass('control-sidebar-open')) {
        $('.control-sidebar').addClass('control-sidebar-open');
    }
}

function closeControlSidebar() {
    // check if sidebar is open and close
    if ($('.control-sidebar').hasClass('control-sidebar-open')) {
        $('.control-sidebar').removeClass('control-sidebar-open');
    }
}

// Used to close opened Main Sidebar after pjax successful request
function openMainSidebar() {
    // check if sidebar is close and open
    if (!$('body').hasClass('sidebar-open')) {
        $('body').hasClass('sidebar-open');
    }
}

function closeMainSidebar() {
    // check if sidebar is open and close
    if ($('body').hasClass('sidebar-open')) {
        $('body').removeClass('sidebar-open');
    }
}

// ------------------------------------------------------------------------------------
// Spinners
// ------------------------------------------------------------------------------------

// Show spinner in .box divs
function showSpin(obj) {
    obj.css('display', 'block');
}

// Hide spinner in .box divs
function hideSpin(obj) {
    obj.css('display', 'none');
}

// actions before an ajax call
function beforeAjaxCall() {
    // show spinner 
    showSpin($('#yodiwo-spinner'));
}

// actions after an ajax call
function afterAjaxCall() {
    // hide spinner
    hideSpin($('#yodiwo-spinner'));
}

// ------------------------------------------------------------------------------------
// Text Helpers
// ------------------------------------------------------------------------------------

// Line count of the provided string
function helperLinesCount(str) {
    try {
        return ((str.match(/[^\n]*\n[^\n]*/gi).length) + 1);
    } catch (e) {
        return 1;
    }
}

// Used for spliting long text fields
function helperSplitToLines(text, maxCharsPerLine) {
    var maxLen = (maxCharsPerLine == 0) ? 10 : maxCharsPerLine;
    // split text to multiple lines containing at least maxLen characters
    var temp = "";
    var index = 0;

    // get first index of whitespace
    var newIndex = text.indexOf(" ");
    if (newIndex == -1)
        return text;

    while (newIndex != -1) {
        // get next word 
        var newText = text.slice(index, index + newIndex);
        index += newIndex + 1; // ignore the whitespace that we just found
        // add to temp the new segment
        temp += newText;
        // add the first new line character after at least maxLen characters
        if (temp.match(/\n/g) == null) {
            if (temp.length > maxLen) {
                temp += "\n";
            }
            else {
                // else add whitespace
                temp += " ";
            }
        }
        else {
            // check if the temp string has a line containing more than maxLen characters
            var lastIndex = temp.lastIndexOf("\n");
            if (lastIndex > 0 && temp.slice(lastIndex, temp.length).length > maxLen) {
                temp += "\n";
            }
            else {
                temp += " ";
            }
        }
        // get next index of whitespace
        var newIndex = text.slice(index, text.length).indexOf(" ");
    }
    // add the rest of the text
    temp += text.slice(index, text.length);
    return he.encode(temp);
}

// Used for spliting long text fields into multiple lines
function helperSplitToMultipleLines(text, maxCharsPerLine) {
    var maxLen = (maxCharsPerLine == 0) ? 10 : maxCharsPerLine;
    // split text to multiple lines containing at least maxLen characters
    var temp = "";
    var index = 0;

    // get first index of whitespace
    var newIndex = text.indexOf(" ");
    if (newIndex == -1)
        return text;

    while (newIndex != -1) {
        // get next word 
        var newText = text.slice(index, index + newIndex);
        index += newIndex + 1; // ignore the whitespace that we just found
        // add to temp the new segment
        temp += newText;
        // add the first new line character after at least maxLen characters
        if (temp.match(/\n/g) == null) {
            if (temp.length > maxLen) {
                temp += "\n";
            }
            else {
                // else add whitespace
                temp += " ";
            }
        }
        else {
            // check if the temp string has a line containing more than maxLen characters
            var lastIndex = temp.lastIndexOf("\n");
            if (lastIndex > 0 && temp.slice(lastIndex, temp.length).length > maxLen) {
                temp += "\n";
            }
            else {
                temp += " ";
            }
        }
        // get next index of whitespace
        var newIndex = text.slice(index, text.length).indexOf(" ");
    }
    // add the rest of the text
    temp += text.slice(index, text.length);
    return temp;
}

function helperRemoveLinesAndEncode(text) {
    return text ? text.replace(/(\r\n|\n|\r)/gm, " ").replace(/'/gm, "") : "";
}

// ------------------------------------------------------------------------------------
// Buttons
// ------------------------------------------------------------------------------------

// Show button in .box divs
function showButton(obj) {
    obj.css('display', 'block');
}

// Hide button in .box divs
function hideButton(obj) {
    obj.css('display', 'none');
}

// Show jquery elem
function showElement(obj) {
    obj.css('display', 'block');
}

// Hide jquery elem
function hideElement(obj) {
    obj.css('display', 'none');
}



// Activate Button
function activateButton(obj) {
    if (obj && !obj.parent().hasClass('active'))
        obj.addClass('active');
}

// DeActivate Button
function deActivateButton(obj) {
    if (obj && obj.hasClass('active'))
        obj.removeClass('active');
}

// ------------------------------------------------------------------------------------
// Yodiwo ToolTip
// ------------------------------------------------------------------------------------

// show tooltip for block ( on hover delay before showing an tooltip)
function ShowYodiwoTooltip(target, text, delay, position, ignoreWords) {
    console.log("ShowTooltip() evt:%O", target);

    // if any button is pressed -> don't display tooltip

    // Set Timeout and after delay msec. display tooltip
    UtilEnv.toolTipInterval = setTimeout(function () {

        // check if all required object are not null
        if (text.length > 0 && UtilEnv.containers.toolboxTooltip != null
			&& UtilEnv.containers.toolboxTooltipArrow != null
			&& UtilEnv.containers.toolboxTooltipText != null) {

            var rect = target.getBoundingClientRect();
            var splittedText = ignoreWords == true ? helperSplitToMultipleLines(text) : helperSplitToLines(text);

            // check if width and height are greater than zero
            if (rect.width != 0 & rect.height != 0) {
                // check optiopositionns and place appropriately the tooltip div
                switch (position) {
                    case 'top':
                        YodiwoTooltipHelperSetPosition('top');
                        UtilEnv.containers.toolboxTooltip.css("left", parseInt(rect.left) + "px");
                        UtilEnv.containers.toolboxTooltip.css("top", parseInt(rect.top + rect.height) + "px");
                        UtilEnv.containers.toolboxTooltip.css("display", "block");
                        UtilEnv.containers.toolboxTooltipArrow.css("left", parseInt(rect.width / 2) + "px");
                        break;
                    case 'bottom':
                        YodiwoTooltipHelperSetPosition('bottom');
                        UtilEnv.containers.toolboxTooltip.css("left", parseInt(rect.left) + "px");
                        UtilEnv.containers.toolboxTooltip.css("top", parseInt(rect.top - rect.height) + "px");
                        UtilEnv.containers.toolboxTooltip.css("display", "block");
                        UtilEnv.containers.toolboxTooltipArrow.css("left", parseInt(rect.width / 2) + "px");

                        break;
                    case 'left':
                        YodiwoTooltipHelperSetPosition('left');
                        UtilEnv.containers.toolboxTooltip.css("left", parseInt($(document).width() - rect.left + rect.width) + "px");
                        UtilEnv.containers.toolboxTooltip.css("top", parseInt(rect.top) + "px");
                        UtilEnv.containers.toolboxTooltip.css("display", "block");
                        break;
                    case 'right':
                        YodiwoTooltipHelperSetPosition('right');
                        UtilEnv.containers.toolboxTooltip.css("right", parseInt($(document).width() - rect.right + rect.width) + "px");
                        UtilEnv.containers.toolboxTooltip.css("top", parseInt(rect.top) + "px");
                        UtilEnv.containers.toolboxTooltip.css("display", "block");
                        break;
                    default:
                        break;
                }
                UtilEnv.containers.toolboxTooltipText.html(splittedText);
            }
            else
                console.log("rect width == heigth == 0 -> don't display tooltip")
        }
    }, delay);
}

function YodiwoTooltipHelperSetPosition(option) {
    if (UtilEnv.containers.toolboxTooltip != null
                && UtilEnv.containers.toolboxTooltipArrow != null
                && UtilEnv.containers.toolboxTooltipText != null) {

        // remove all dynamically assigned styles 
        UtilEnv.containers.toolboxTooltip.removeAttr('style');
        UtilEnv.containers.toolboxTooltipArrow.removeAttr('style');

        // remove all classes related to positioning
        UtilEnv.containers.toolboxTooltip.removeClass('top');
        UtilEnv.containers.toolboxTooltip.removeClass('bottom');
        UtilEnv.containers.toolboxTooltip.removeClass('left');
        UtilEnv.containers.toolboxTooltip.removeClass('right');

        // add option if valid
        if (option == 'top' || option == 'bottom' || option == 'left' || option == 'right')
            UtilEnv.containers.toolboxTooltip.addClass(option);
    }
}

// hide tooltip or cleartimeout for block
function HideYodiwoTooltip() {
    if (UtilEnv.containers.toolboxTooltip != null) {
        // clearTimeout
        clearTimeout(UtilEnv.toolTipInterval);
        UtilEnv.containers.toolboxTooltip.css("display", "none");
    }
}

function createTooltipHTML(title, placement, onclick) {
    return 'style="cursor:pointer" onclick="' + onclick + '" data-toggle="tooltip" data-placement="' + placement + '" title="' + title + '"'
}

// ------------------------------------------------------------------------------------
// DateTime
// ------------------------------------------------------------------------------------

// DateTime Javascript object generation
function generateValidDateTimeString(nonValidDateTimeString) {
    // the date will be have the format: "2015-09-08T00:00:00Z" -> convert to Javascript Date  Object
    var dayIndex = nonValidDateTimeString.lastIndexOf('-');
    var timeIndexLast = nonValidDateTimeString.lastIndexOf(':');
    var timeIndexFirst = nonValidDateTimeString.indexOf('T');
    // get the format yyyy-mm-dd
    var validDateString = nonValidDateTimeString.slice(0, dayIndex + 3);
    var validTimeString = nonValidDateTimeString.slice(timeIndexFirst + 1, timeIndexLast + 3);
    // split 
    var dateSplits = validDateString.split('-');
    var timeSplits = validTimeString.split(':');
    // parseInt and get year, month and date
    var year = parseInt(dateSplits[0], 10);
    var month = parseInt(dateSplits[1], 10);
    var date = parseInt(dateSplits[2], 10);

    var hour = parseInt(timeSplits[0], 10);
    var minutes = parseInt(timeSplits[1], 10);
    var second = parseInt(timeSplits[2], 10);

    // (month - 1) because month range is [0, 11]
    return new Date(year, month - 1, date, hour, minutes, second);
}

// ------------------------------------------------------------------------------------
// Jquery
// ------------------------------------------------------------------------------------
// Load scripts and use cached files if they exist

jQuery.getCachedScript = function (url, options) {

    // Allow user to set any option except for dataType, cache, and url
    options = $.extend(options || {}, {
        dataType: "script",
        cache: true,
        url: url
    });

    // Use $.ajax() since it is more flexible than $.getScript
    // Return the jqXHR object so we can chain callbacks
    return jQuery.ajax(options);
};

// Regex Selector for jQuery
jQuery.expr[':'].regex = function (elem, index, match) {
    var matchParams = match[3].split(','),
        validLabels = /^(data|css):/,
        attr = {
            method: matchParams[0].match(validLabels) ?
                        matchParams[0].split(':')[0] : 'attr',
            property: matchParams.shift().replace(validLabels, '')
        },
        regexFlags = 'ig',
        regex = new RegExp(matchParams.join('').replace(/^s+|s+$/g, ''), regexFlags);
    return regex.test(jQuery(elem)[attr.method](attr.property));
}