// Đồng hồ đếm ngược hạn chót và kéo thả thẻ trên bảng Kanban.
(function ($) {
    'use strict';

    // ---------- Đồng hồ đếm ngược ----------

    // Đổi số mili giây thành chuỗi như "2 ngày 3 giờ"
    function formatDuration(milliseconds) {
        var totalMinutes = Math.floor(Math.abs(milliseconds) / 60000);
        var days = Math.floor(totalMinutes / 1440);
        var hours = Math.floor((totalMinutes % 1440) / 60);
        var minutes = totalMinutes % 60;

        if (days > 0) {
            return days + ' ngày ' + hours + ' giờ';
        }
        if (hours > 0) {
            return hours + ' giờ ' + minutes + ' phút';
        }
        return minutes + ' phút';
    }

    function updateCountdowns() {
        $('.countdown').each(function () {
            var element = $(this);
            var dueText = element.attr('data-due');
            if (!dueText) {
                return;
            }

            // Thẻ đã nằm ở cột hoàn thành thì không đếm ngược nữa
            if (element.attr('data-done') === '1') {
                element.text('Đã hoàn thành').attr('class', 'countdown countdown-done');
                return;
            }

            var difference = new Date(dueText).getTime() - new Date().getTime();
            if (difference < 0) {
                element.text('Quá hạn ' + formatDuration(difference)).attr('class', 'countdown countdown-overdue');
            } else if (difference < 24 * 60 * 60 * 1000) {
                element.text('Còn ' + formatDuration(difference)).attr('class', 'countdown countdown-soon');
            } else {
                element.text('Còn ' + formatDuration(difference)).attr('class', 'countdown');
            }
        });
    }

    // ---------- Kéo thả thẻ ----------

    function updateColumnCounts() {
        $('.board-column').each(function () {
            var column = $(this);
            column.find('.column-count').text(column.find('.task-card').length);
        });
    }

    // Tìm thẻ nằm ngay sau vị trí con trỏ chuột để biết cần chèn thẻ đang kéo vào đâu
    function findCardAfter(list, mouseY) {
        var cards = list.querySelectorAll('.task-card:not(.dragging)');
        var closestCard = null;
        var closestOffset = Number.NEGATIVE_INFINITY;

        for (var i = 0; i < cards.length; i++) {
            var box = cards[i].getBoundingClientRect();
            var offset = mouseY - box.top - box.height / 2;
            if (offset < 0 && offset > closestOffset) {
                closestOffset = offset;
                closestCard = cards[i];
            }
        }
        return closestCard;
    }

    function initBoard() {
        var boardElement = document.getElementById('board');
        if (!boardElement) {
            return;
        }

        var board = $(boardElement);
        var moveUrl = board.attr('data-move-url');
        var token = $('#antiForgeryForm input[name="__RequestVerificationToken"]').val();
        var draggedCard = null;

        board.on('dragstart', '.task-card', function (e) {
            draggedCard = this;
            this.classList.add('dragging');
            e.originalEvent.dataTransfer.effectAllowed = 'move';
            // Firefox yêu cầu phải gọi setData thì thao tác kéo mới bắt đầu
            e.originalEvent.dataTransfer.setData('text/plain', this.getAttribute('data-task-id'));
        });

        board.on('dragend', '.task-card', function () {
            this.classList.remove('dragging');
            draggedCard = null;
        });

        // Khi kéo qua một danh sách thẻ, di chuyển thẻ đang kéo vào đúng vị trí để người dùng thấy trước kết quả
        board.on('dragover', '.task-list', function (e) {
            if (draggedCard === null) {
                return;
            }
            e.preventDefault();
            e.originalEvent.dataTransfer.dropEffect = 'move';

            var cardAfter = findCardAfter(this, e.originalEvent.clientY);
            if (cardAfter === null) {
                this.appendChild(draggedCard);
            } else {
                this.insertBefore(draggedCard, cardAfter);
            }
        });

        // Khi thả: gửi cột mới và vị trí mới lên máy chủ để lưu vào cơ sở dữ liệu
        board.on('drop', '.task-list', function (e) {
            e.preventDefault();
            if (draggedCard === null) {
                return;
            }

            var list = $(this);
            var card = draggedCard;
            var isDone = list.attr('data-is-done') === '1';
            var data = {
                __RequestVerificationToken: token,
                taskId: $(card).attr('data-task-id'),
                columnId: list.attr('data-column-id'),
                index: list.children('.task-card').index(card)
            };

            $.post(moveUrl, data)
                .done(function (result) {
                    if (result && result.success) {
                        $(card).find('.countdown').attr('data-done', isDone ? '1' : '0');
                        updateCountdowns();
                        updateColumnCounts();
                    } else {
                        window.alert(result && result.message ? result.message : 'Không thể di chuyển thẻ.');
                        window.location.reload();
                    }
                })
                .fail(function () {
                    window.alert('Không thể lưu vị trí thẻ. Trang sẽ được tải lại.');
                    window.location.reload();
                });
        });
    }

    $(function () {
        updateCountdowns();
        window.setInterval(updateCountdowns, 30000);
        initBoard();
    });
})(jQuery);
