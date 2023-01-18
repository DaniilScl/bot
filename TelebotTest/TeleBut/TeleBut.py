import telebot;

bot = telebot.TeleBot('5840461725:AAHny1RnwlteV9BFlk-1IZ8ebNEyrF5lqKo');

from telebot.types import ReplyKeyboardMarkup, InlineKeyboardMarkup, InlineKeyboardButton

markup = InlineKeyboardMarkup()
markup.add(InlineKeyboardButton(text='Скрыть', callback_data='unseen'))
bot.send_message(m.from_user.id, "Привет", reply_markup = markup)

@bot.callback_query_handler(func=lambda call:True)
def callback_query(call):
    req = call.data.split('_')

    if req[0] == 'unseen':
        bot.delete_message(call.message.chat.id, call.message.message_id)
